using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Settings
{
    public enum Param : byte
    {
        None = 0,
        Statistics = 1,
        History = 2,
        Offline = 3,
        Covid = 4
    }

    static bool[] Params;

    static Settings()
    {
        Params = new bool[] {false, false, false, false, false};

        if (PlayerPrefs.HasKey("settings"))
        {
            string str = PlayerPrefs.GetString("settings");
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (i < Params.Length)
                    Params[i] = str[i] == '1';
            }
        }
    }

    public static bool GetValue(Param pParam)
    {
        byte v = (byte) pParam;
        if (v < Params.Length)
            return Params[v];
        return false;
    }

    public static void SetValue(Param pParam, bool pValue)
    {
        byte v = (byte) pParam;
        if (v < Params.Length)
            Params[v] = pValue;

        string str = "";
        foreach (var t in Params)
            str += t ? "1" : "0";
            
        PlayerPrefs.SetString("settings", str);
        PlayerPrefs.Save();
    }
}