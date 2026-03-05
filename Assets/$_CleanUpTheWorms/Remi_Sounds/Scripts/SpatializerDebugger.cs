using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpatializerDebugger : MonoBehaviour
{
    public TMP_Text debugText; // Assign a UI Text element if you have one, or it will use GUI

    void Start()
    {
        StartCoroutine(CheckAudioStatus());
    }

    IEnumerator CheckAudioStatus()
    {
        // Wait a few seconds for the engine to initialize
        yield return new WaitForSeconds(2);

        string report = "--- AUDIO DEBUG REPORT ---\n";

        // 1. Check Active Spatializer
        report += $"Active Plugin: {AudioSettings.GetSpatializerPluginName()}\n";

        // 2. Check Sample Rate (Must be 48000 for Meta)
        report += $"Sample Rate: {AudioSettings.outputSampleRate}Hz\n";

        // 3. Check for AudioSource
        AudioSource src = FindFirstObjectByType<AudioSource>();
        if (src != null)
        {
            report += $"Source Found: {src.gameObject.name}\n";
            report += $"Spatialize Enabled: {src.spatialize}\n";
            report += $"Spatial Blend (Must be 1): {src.spatialBlend}\n";
            report += $"Is Playing: {src.isPlaying}\n";
            report += $"Mute State: {src.mute}\n";
        }
        else
        {
            report += "No AudioSource found in scene!\n";
        }

        Debug.Log(report);
        if (debugText != null) debugText.text = report;
    }

    // Displays the report on the screen in the build
    void OnGUI()
    {
        GUI.color = Color.yellow;
        GUI.Label(new Rect(10, 10, 500, 500), "Check console or assign UI Text for full report.");
    }
}