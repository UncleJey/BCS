using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Toggler : MonoBehaviour
{
    [SerializeField] private Sprite ok, no;

    private Button btn;
    [SerializeField] private Image img;

    public Settings.Param param;

    /// <summary>
    /// Описание действия
    /// </summary>
    public string readme;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);
    }

    private void OnEnable()
    {
        Value = Settings.GetValue(param);
    }

    public bool Value
    {
        get => img.sprite.Equals(ok);
        set => img.sprite = value ? ok : no;
    }

    private void OnDestroy()
    {
        btn.onClick.RemoveAllListeners();
    }

    void OnBtnClick()
    {
        bool newVal = !Value;
        Value = newVal;
        Settings.SetValue(param, newVal);
    }
}