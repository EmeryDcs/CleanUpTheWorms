using UnityEngine;
using NAudio.CoreAudioApi;
using System;
using System.Linq;

public class NAudioDeviceLister : MonoBehaviour
{
    void Awake()
    {
    }

    void Start()
    {
        if (!enabled) return;

        var enumerator = new MMDeviceEnumerator();
        var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

        Debug.Log("=== ACTIVE AUDIO DEVICES ===");
        foreach (var device in devices)
        {
            Debug.Log($"Device Name: {device.FriendlyName}");
        }
    }
}