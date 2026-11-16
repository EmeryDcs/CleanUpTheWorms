using System;
using System.Globalization;
using UnityEngine;

public static class S_LaunchSettings
{
    private const string KeySaved = "LS_Saved";
    private const string KeyEnglish = "LS_English";
    private const string KeyJapanese = "LS_Japanese";
    private const string KeyHaptics = "LS_Haptics";
    private const string KeyFast = "LS_Fast";
    private const string KeyWithDevice = "LS_WithDevice";
    private const string KeyHasMin = "LS_HasMin";
    private const string KeyMin = "LS_Min";
    private const string KeyHasMax = "LS_HasMax";
    private const string KeyMax = "LS_Max";

    private static bool initialized;

    // Valeurs de base (aucun argument et rien de sauvegarde)
    private static bool isEnglish = false;
    private static bool isJapanese = false;
    private static bool haptics = false;
    private static bool fastVersion = true;
    private static bool withDevice = false;
    private static bool hasMin = false;
    private static float minTaille = 0f;
    private static bool hasMax = false;
    private static float maxTaille = 0f;

    public static bool IsEnglish { get { Init(); return isEnglish; } }
    public static bool IsJapanese { get { Init(); return isJapanese; } }
    public static bool Haptics { get { Init(); return haptics; } }
    public static bool FastVersion { get { Init(); return fastVersion; } }
    public static bool WithDevice { get { Init(); return withDevice; } }
    public static bool HasMinTaille { get { Init(); return hasMin; } }
    public static float MinTaille { get { Init(); return minTaille; } }
    public static bool HasMaxTaille { get { Init(); return hasMax; } }
    public static float MaxTaille { get { Init(); return maxTaille; } }

    // Evite de garder l'etat si le domain reload est desactive dans l'editeur
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        initialized = false;
        isEnglish = false;
        isJapanese = false;
        haptics = false;
        fastVersion = true;
        withDevice = false;
        hasMin = false;
        minTaille = 0f;
        hasMax = false;
        maxTaille = 0f;
    }

    private static void Init()
    {
        if (initialized) return;
        initialized = true;

        bool launcherUsed = false;
        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-launcher":
                    launcherUsed = true;
                    break;
                case "-isinenglish":
                    isEnglish = true;
                    launcherUsed = true;
                    break;
                case "-isjapanese":
                    isJapanese = true;
                    launcherUsed = true;
                    break;
                case "-haptics":
                    haptics = true;
                    launcherUsed = true;
                    break;
                case "-fastversion":
                    fastVersion = true;
                    launcherUsed = true;
                    break;
                case "-nofastversion":
                    fastVersion = false;
                    launcherUsed = true;
                    break;
                case "-withdevice":
                    withDevice = true;
                    launcherUsed = true;
                    break;
                case "-mintaille":
                    launcherUsed = true;
                    if (i + 1 < args.Length && float.TryParse(args[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out float min))
                    {
                        hasMin = true;
                        minTaille = min;
                    }
                    break;
                case "-maxtaille":
                    launcherUsed = true;
                    if (i + 1 < args.Length && float.TryParse(args[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out float max))
                    {
                        hasMax = true;
                        maxTaille = max;
                    }
                    break;
            }
        }

        if (launcherUsed)
        {
            Save();
        }
        else if (PlayerPrefs.GetInt(KeySaved, 0) == 1)
        {
            Load();
        }
        // sinon : valeurs de base
    }

    private static void Save()
    {
        PlayerPrefs.SetInt(KeySaved, 1);
        PlayerPrefs.SetInt(KeyEnglish, isEnglish ? 1 : 0);
        PlayerPrefs.SetInt(KeyJapanese, isJapanese ? 1 : 0);
        PlayerPrefs.SetInt(KeyHaptics, haptics ? 1 : 0);
        PlayerPrefs.SetInt(KeyFast, fastVersion ? 1 : 0);
        PlayerPrefs.SetInt(KeyWithDevice, withDevice ? 1 : 0);
        PlayerPrefs.SetInt(KeyHasMin, hasMin ? 1 : 0);
        PlayerPrefs.SetFloat(KeyMin, minTaille);
        PlayerPrefs.SetInt(KeyHasMax, hasMax ? 1 : 0);
        PlayerPrefs.SetFloat(KeyMax, maxTaille);
        PlayerPrefs.Save();
    }

    private static void Load()
    {
        isEnglish = PlayerPrefs.GetInt(KeyEnglish, 0) == 1;
        isJapanese = PlayerPrefs.GetInt(KeyJapanese, 0) == 1;
        haptics = PlayerPrefs.GetInt(KeyHaptics, 0) == 1;
        fastVersion = PlayerPrefs.GetInt(KeyFast, 1) == 1;
        withDevice = PlayerPrefs.GetInt(KeyWithDevice, 0) == 1;
        hasMin = PlayerPrefs.GetInt(KeyHasMin, 0) == 1;
        minTaille = PlayerPrefs.GetFloat(KeyMin, 0f);
        hasMax = PlayerPrefs.GetInt(KeyHasMax, 0) == 1;
        maxTaille = PlayerPrefs.GetFloat(KeyMax, 0f);
    }

    // Pour repartir des valeurs de base au prochain lancement sans launcher
    public static void ClearSaved()
    {
        PlayerPrefs.DeleteKey(KeySaved);
        PlayerPrefs.Save();
    }
}