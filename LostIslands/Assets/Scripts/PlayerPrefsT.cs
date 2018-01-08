using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TempPrefs //Temporäre PlayerPrefs (werden bei stopp gelöscht) wie variable die man aber von überall verändern kann
{

    public static List<string> TempBoolList = new List<string>();
    public static List<string> TempIntList = new List<string>();
    public static List<string> TempStringList = new List<string>();


    public static void SetBool(String name, bool value)
    {
        AddtoTempBoolList(name);

        PlayerPrefs.SetInt("Temp_" + name, value ? 1 : 0);
    }

    public static bool GetBool(String name)
    {
        AddtoTempBoolList(name);
        if (PlayerPrefs.GetInt("Temp_" + name) == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public static void SetInt(String name, int value)
    {
        AddtoTempIntList(name);
        PlayerPrefs.SetInt("Temp_" + name, value);
    }

    public static int GetInt(String name)
    {
        AddtoTempIntList(name);
        return PlayerPrefs.GetInt("Temp_" + name);
    }


    public static void SetString(String name, string text)
    {
        AddtoTempStringList(name);
        PlayerPrefs.SetString("Temp_" + name, text);
    }

    public static string GetString(String name)
    {
        AddtoTempStringList(name);
        return PlayerPrefs.GetString("Temp_" + name);
    }


    //##############################################################################
    static void AddtoTempStringList(string name)
    {
        if (!TempStringList.Contains("Temp_" + name)) //nur wenn noch nicht drin
        {
            TempStringList.Add("Temp_" +name);
        }
    }

    static void AddtoTempBoolList(string name)
    {
        if (!TempBoolList.Contains("Temp_" + name))
        {
            TempBoolList.Add("Temp_" + name);
        }
    }

    static void AddtoTempIntList(string name)
    {
        if (!TempIntList.Contains("Temp_" + name))
        {
            TempIntList.Add("Temp_" + name);
        }
    }

    //##############################################################################

    public static void DeleteAll() //alle TempPrefs löschen
    {
        foreach(string Stringname in TempBoolList)
        {
            PlayerPrefs.SetInt(Stringname, 0);
        }

        foreach (string Stringname in TempStringList)
        {
            PlayerPrefs.SetString(Stringname, "");
        }

        foreach (string Stringname in TempIntList)
        {
            PlayerPrefs.SetInt(Stringname, 0);
        }


    }
}

public class PlayerPrefsT : MonoBehaviour {


    void OnApplicationQuit() //wenn aus Spiel raus geht 
    {
        TempPrefs.DeleteAll();
    }


}
