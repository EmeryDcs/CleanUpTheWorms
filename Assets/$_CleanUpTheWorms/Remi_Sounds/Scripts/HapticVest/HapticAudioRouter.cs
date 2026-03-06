using UnityEngine;
using NAudio.Wave;
using System;
using System.Runtime.InteropServices;

[RequireComponent(typeof(AudioSource))]
public class HapticAudioRouter : MonoBehaviour
{
    public string targetDeviceName = "KOR-FX";

    [Range(0f, 5f)]
    public float volumeMultiplier = 1.0f;

    private BufferedWaveProvider waveProvider;
    private WaveOutEvent waveOut;

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
    private struct WAVEOUTCAPS
    {
        public short wMid;
        public short wPid;
        public int vDriverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;
        public uint dwFormats;
        public short wChannels;
        public short wReserved1;
        public int dwSupport;
    }

    [DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int waveOutGetNumDevs();

    [DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int waveOutGetDevCaps(int uDeviceID, out WAVEOUTCAPS pwoc, int cbwoc);

    void Start()
    {
        int targetDeviceNumber = -1;
        int deviceCount = waveOutGetNumDevs();

        for (int i = 0; i < deviceCount; i++)
        {
            WAVEOUTCAPS caps;
            if (waveOutGetDevCaps(i, out caps, Marshal.SizeOf(typeof(WAVEOUTCAPS))) == 0)
            {
                if (caps.szPname.IndexOf(targetDeviceName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    targetDeviceNumber = i;
                    break;
                }
            }
        }

        if (targetDeviceNumber == -1)
        {
            Debug.LogError($"[Haptic Router] Could not find an active audio device containing the name: {targetDeviceName}");
            return;
        }

        WAVEOUTCAPS selectedCaps;
        waveOutGetDevCaps(targetDeviceNumber, out selectedCaps, Marshal.SizeOf(typeof(WAVEOUTCAPS)));
        Debug.Log($"[Haptic Router] Successfully routed haptics to: {selectedCaps.szPname} (Device {targetDeviceNumber})");

        int sampleRate = AudioSettings.outputSampleRate;
        waveProvider = new BufferedWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2));

        waveProvider.BufferDuration = TimeSpan.FromMilliseconds(50);
        waveProvider.DiscardOnBufferOverflow = true;

        waveOut = new WaveOutEvent();
        waveOut.DeviceNumber = targetDeviceNumber;
        waveOut.DesiredLatency = 50;
        waveOut.NumberOfBuffers = 2;
        waveOut.Init(waveProvider);
        waveOut.Play();
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (waveProvider == null) return;



        if (Mathf.Abs(volumeMultiplier - 1.0f) > 0.001f)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= volumeMultiplier;
            }
        }

        byte[] byteBuffer = new byte[data.Length * 4];
        Buffer.BlockCopy(data, 0, byteBuffer, 0, byteBuffer.Length);

        waveProvider.AddSamples(byteBuffer, 0, byteBuffer.Length);

        Array.Clear(data, 0, data.Length);
    }

    void OnDestroy()
    {
        if (waveOut != null)
        {
            waveOut.Stop();
            waveOut.Dispose();
        }
    }
}