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

    private WaveOutEvent waveOut;
    private AudioClipWaveProvider waveProvider;
    private AudioSource audioSource;
    private bool wasPlaying = false;

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
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = true;

        if (audioSource.clip != null)
        {
            waveProvider = new AudioClipWaveProvider(audioSource.clip);
        }

        InitializeAudio();
    }

    void InitializeAudio()
    {
        if (waveOut != null)
        {
            waveOut.Stop();
            waveOut.Dispose();
            waveOut = null;
        }

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

        waveOut = new WaveOutEvent();
        waveOut.DeviceNumber = targetDeviceNumber;

        if (waveProvider != null)
        {
            waveOut.Init(waveProvider);
        }
    }

    void Update()
    {
        if (waveProvider != null)
        {
            waveProvider.Volume = volumeMultiplier;
            waveProvider.Loop = audioSource.loop;
        }

        if (audioSource.isPlaying && !wasPlaying)
        {
            if (waveOut != null && waveProvider != null)
            {
                waveProvider.SetPosition(audioSource.timeSamples * audioSource.clip.channels);
                waveOut.Play();
            }
            wasPlaying = true;
        }
        else if (!audioSource.isPlaying && wasPlaying)
        {
            if (waveOut != null)
            {
                waveOut.Stop();
            }
            wasPlaying = false;
        }
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

public class AudioClipWaveProvider : IWaveProvider
{
    private float[] audioData;
    private int position;

    public float Volume { get; set; } = 1.0f;
    public bool Loop { get; set; } = false;
    public WaveFormat WaveFormat { get; private set; }

    public AudioClipWaveProvider(AudioClip clip)
    {
        audioData = new float[clip.samples * clip.channels];
        clip.GetData(audioData, 0);
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(clip.frequency, clip.channels);
    }

    public void SetPosition(int samplePosition)
    {
        position = Mathf.Clamp(samplePosition, 0, audioData.Length);
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        int bytesPerSample = 4;
        int samplesToRead = count / bytesPerSample;
        int samplesAvailable = audioData.Length - position;
        int samplesToCopy = Math.Min(samplesToRead, samplesAvailable);

        if (samplesToCopy <= 0)
        {
            if (Loop)
            {
                position = 0;
                samplesAvailable = audioData.Length;
                samplesToCopy = Math.Min(samplesToRead, samplesAvailable);
                if (samplesToCopy <= 0) return 0;
            }
            else
            {
                return 0;
            }
        }

        float[] tempFloats = new float[samplesToCopy];
        Array.Copy(audioData, position, tempFloats, 0, samplesToCopy);

        if (Mathf.Abs(Volume - 1.0f) > 0.001f)
        {
            for (int i = 0; i < tempFloats.Length; i++)
            {
                tempFloats[i] *= Volume;
            }
        }

        Buffer.BlockCopy(tempFloats, 0, buffer, offset, tempFloats.Length * bytesPerSample);

        position += samplesToCopy;
        return samplesToCopy * bytesPerSample;
    }
}