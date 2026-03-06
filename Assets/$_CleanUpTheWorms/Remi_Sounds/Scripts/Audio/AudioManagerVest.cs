using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManagerVest : MonoBehaviour
{
    public static AudioManagerVest Instance;

    [Header("Global Vest Sounds")]
    public List<Sound> vestSounds;

    [Header("Vest Settings")]
    [SerializeField] private AudioMixerGroup vestMixerGroup;

    private AudioSource vestSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        vestSource = gameObject.GetComponent<AudioSource>();
        if (vestSource == null)
        {
            vestSource = gameObject.AddComponent<AudioSource>();
        }

        vestSource.outputAudioMixerGroup = vestMixerGroup;
        vestSource.spatialBlend = 0f;

        foreach (Sound s in vestSounds)
        {
            if (s.playOnAwake)
            {
                PlayGlobalVestSound(s.name);
            }
        }
    }

    public void PlayVestSound(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        vestSource.clip = clip;
        vestSource.volume = volume;
        vestSource.spatialBlend = 0f;
        vestSource.Play();
    }

    public void PlayGlobalVestSound(string name)
    {
        Sound s = vestSounds.Find(sound => sound.name == name);
        if (s == null) return;

        vestSource.clip = s.clip;
        vestSource.volume = s.volume;
        vestSource.pitch = s.pitch;
        vestSource.loop = s.loop;
        vestSource.spatialBlend = 0f;

        vestSource.Play();
    }

    public void StopGlobalVestSound(string name)
    {
        Sound s = vestSounds.Find(sound => sound.name == name);
        if (s != null && vestSource.clip == s.clip)
        {
            vestSource.Stop();
            vestSource.clip = null;
        }
    }
}