using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private UIHider settingsView;
    [SerializeField] private UIHider buttonsView;

    private static UIMenu _instance;

    private void Awake()
    {
        _instance = this;
        MainScreen();
    }

    private async void Start()
    {
        //JsonObject j = await CovidParser.Parse("https://www.gosuslugi.ru/covid-cert/verify/9710000025717304?lang=ru&ck=f2ab62b1a16724942b19e836a83c5258");
        CovidParser.ParseAddress("https://www.gosuslugi.ru/api/covid-cert/v3/cert/check/9710000025717304?lang=ru&ck=f2ab62b1a16724942b19e836a83c5258", out string num, out string ck);
        string s1 = CovidParser.EnCode(num, ck);
        Debug.Log(s1);
        string s2 = CovidParser.DeCode(s1);
        Debug.Log(s2);
        //Debug.Log("num "+num+" ck "+ck);
    }

    public static void MainScreen()
    {
        _instance.settingsView.Visible = false;
        _instance.buttonsView.Visible = true;
    }

    public static void SettingsScreen()
    {
        _instance.settingsView.Visible = true;
        _instance.buttonsView.Visible = false;
    }
}