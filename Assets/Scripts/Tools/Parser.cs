using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public static class CovidParser
{
    public static async Task<JsonObject> Parse(string src)
    {
        /*
        string src =
            "https://www.gosuslugi.ru/covid-cert/verify/9710000025717304?lang=ru&ck=f2ab62b1a16724942b19e836a83c5258";
        */

        string addr = src.Replace("covid-cert/verify", "api/covid-cert/v3/cert/check");
        //"https://www.gosuslugi.ru/api/covid-cert/v3/cert/check/9710000025717304?lang=ru&ck=f2ab62b1a16724942b19e836a83c5258";
        JsonObject result = new JsonObject();

        void Add(string pName, string pValue)
        {
            if (string.IsNullOrWhiteSpace(pName) || string.IsNullOrWhiteSpace(pValue))
                return;

            result.Add(pName, pValue);
        }

        UnityWebRequest req =
            UnityWebRequest.Get(addr);
        req.SendWebRequest();

        while (!req.isDone)
        {
            await Task.Delay(100);
        }

        if (req.isHttpError || req.isNetworkError)
        {
            return new JsonObject {{"e", req.isNetworkError ? 1 : 2}, {"w", req.isNetworkError ? "net" : "html"}};
        }

        if (string.IsNullOrWhiteSpace(req.downloadHandler.text))
        {
            return new JsonObject {{"e", 3}, {"w", "txt"}};
        }

        JsonObject j = JsonObject.FromJson(req.downloadHandler.text);

        if (j == null)
            return new JsonObject {{"e", 4}, {"w", "j1"}};

        JsonArray attrs = j.Get<JsonArray>("items");

        if (attrs == null || attrs.Count < 1)
            return new JsonObject {{"e", 5}, {"w", "a1"}};

        JsonObject item = attrs[0] as JsonObject;
        if (item == null)
            return new JsonObject {{"err", 6}, {"w", "i1"}};

        Add("expiredAt", item.Get<string>("expiredAt"));
        Add("status", item.Get<string>("status"));
        Add("type", item.Get<string>("type"));

        attrs = item.Get<JsonArray>("attrs");
        if (attrs == null || attrs.Count < 1)
            return new JsonObject {{"err", 7}, {"w", "a2"}};

        foreach (JsonObject o in attrs)
        {
            if (o != null)
                Add(o.Get<string>("type"), o.Get<string>("value"));
        }

        string res = "";
        foreach (string k in result.Keys)
        {
            res += $"{k}: {result[k]}\r\n";
        }
        
        Debug.Log(res);
        return result;
    }

    static List<string> SplitMulti(this string val, string seps)
    {
        List<string> res = new List<string>();

        int cnt = val.Length;
        string cur = "";
        for (int i = 0; i < cnt; i++)
        {
            if (seps.IndexOf(val[i]) >= 0)
            {
                if (!string.IsNullOrWhiteSpace(cur))
                {
                    res.Add(cur);
                    cur = "";
                }
            }
            else
            {
                cur += val[i];
            }
        }
        if (!string.IsNullOrWhiteSpace(cur))
            res.Add(cur);

        return res;
    }
    
    public static void ParseAddress(string pAddress, out string num, out string ck)
    {
        num = string.Empty;
        ck = string.Empty;
        
        List<string> data = pAddress.SplitMulti(":/?&=");
        for (int i = data.Count-1; i>=0; i--)
        {
            if (data[i] == "check")
                num = data[i + 1];
            else if (data[i] == "ck")
                ck = data[i + 1];
        }
    }

    private static string bts = "0d19b2f8c37a46e5l";
    private static readonly int[] Num =
    {
        39, 12, 4, 47, 34, 46, 24, 20, 11, 36, 49, 19, 41, 6, 2, 26, 48, 42, 43, 29, 28, 21, 35, 10, 27, 22, 7, 32, 38, 25, 17, 31, 15, 40, 16, 5, 30, 13, 37, 9, 33, 3, 44, 0, 14, 18, 8, 23, 1, 45
    };
    
    public static string EnCode(string a1, string a2)
    {
        string a = a1 + "l" +a2;
        while (a.Length < Num.Length)
        {
            a += "l";
        }
        char[] res = new char[Num.Length];
        int cnt = bts.Length-1;
        for (int i=0; i<a.Length; i++)
        {
            int p = bts.IndexOf(a[i]);
            res[Num[i]] = bts[cnt - p];
        }
        
        return res.ArrayToString();
    }

    public static string DeCode(string a1)
    {
        string a = a1;
        char[] res = new char[Num.Length];
        int cnt = bts.Length-1;
        for (int i=0; i<a.Length; i++)
        {
            int p = bts.IndexOf(a[Num[i]]);
            res[i] = bts[cnt - p];
        }
        
        return res.ArrayToString().Replace("l"," ");
    }
    
    static void SaveFile(string data)
    {
        string destination = Application.persistentDataPath + "/save.txt";
        Debug.Log(destination);
        Debug.Log(data);
        File.WriteAllText(destination, data);
        
    }
}