using System.Collections;
using UnityEngine;

public class AudioRebooter : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Wait for Meta XR and Steam Audio to finish their conflicting initialization
        yield return new WaitForSeconds(1.5f);

        // Force Unity to reset the audio engine using your explicit Project Settings
        AudioSettings.Reset(AudioSettings.GetConfiguration());
    }
}