using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wizcorp.Utils.Logger;

public class SimpleDemo : MonoBehaviour
{
    private IScanner BarcodeScanner;
    public Text TextHeader;
    public RawImage Image;
    public AudioSource Audio;

    [SerializeField] private Status status;
    [SerializeField] private Enabler buttons;
    [SerializeField] private Button stopBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button backBtn;

    void Awake()
    {
        Application.targetFrameRate = 15;

        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        startBtn.onClick.AddListener(ClickStart);
        stopBtn.onClick.AddListener(ClickStop);
        backBtn.onClick.AddListener(ClickBack);
    }

    private void OnDestroy()
    {
        startBtn.onClick.RemoveAllListeners();
        stopBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();

        BarcodeScanner?.Destroy();
    }

    void Start()
    {
        // Create a basic scanner
        BarcodeScanner = new Scanner();
        BarcodeScanner.Camera.Play();
        buttons.Val = (byte) Enabler.State.Pause;

        // Display the camera texture through a RawImage
        BarcodeScanner.OnReady += (sender, arg) =>
        {
            // Set Orientation & Texture
            Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
            Image.transform.localScale = BarcodeScanner.Camera.GetScale();
            Image.texture = BarcodeScanner.Camera.Texture;

            // Keep Image Aspect Ratio
            var rect = Image.GetComponent<RectTransform>();
            var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
        };
/*
        // Track status of the scanner
        BarcodeScanner.StatusChanged += (sender, arg) =>
        {
            TextHeader.text = "Status: " + BarcodeScanner.Status;
            
        };
        */
    }

    void Update()
    {
        BarcodeScanner?.Update();
    }

    #region UI Buttons

    public void ClickStart()
    {
        if (BarcodeScanner == null)
        {
            Log.Warning("No valid camera - Click Start");
            return;
        }

        buttons.Val = (byte) Enabler.State.Play;

        // Start Scanning
        BarcodeScanner.Scan((barCodeType, barCodeValue) =>
        {
            BarcodeScanner.Stop();
            TryParse(barCodeValue);
            // Feedback
            Audio.Play();

#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        });
    }

    async void TryParse(string pValue)
    {
        if (pValue.Contains("/covid-cert/"))
        {
            JsonObject res = await CovidParser.Parse(pValue);
            
        }
        else
            TextHeader.text =  pValue;
    }

    void ClickStop()
    {
        if (BarcodeScanner == null)
        {
            Log.Warning("No valid camera - Click Stop");
            return;
        }

        buttons.Val = (byte) Enabler.State.Pause;

        // Stop Scanning
        BarcodeScanner.Stop();
    }

    void ClickBack()
    {
        // Try to stop the camera before loading another scene
        StartCoroutine(StopCamera(() => { SceneManager.LoadScene("Boot"); }));
    }

    /// <summary>
    /// This coroutine is used because of a bug with unity (http://forum.unity3d.com/threads/closing-scene-with-active-webcamtexture-crashes-on-android-solved.363566/)
    /// Trying to stop the camera in OnDestroy provoke random crash on Android
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    IEnumerator StopCamera(Action callback)
    {
        // Stop Scanning
        Image = null;
        BarcodeScanner.Destroy();
        BarcodeScanner = null;

        // Wait a bit
        yield return new WaitForSeconds(0.1f);

        callback.Invoke();
    }

    #endregion
}