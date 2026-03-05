using UnityEngine;
using NAudio.CoreAudioApi;

public class NAudioDeviceLister : MonoBehaviour
{
    void Start()
    {
        var enumerator = new MMDeviceEnumerator();
        // Look only for active playback/render devices
        var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

        Debug.Log("=== ACTIVE AUDIO DEVICES ===");
        foreach (var device in devices)
        {
            Debug.Log($"Device Name: {device.FriendlyName}");
        }
    }
}