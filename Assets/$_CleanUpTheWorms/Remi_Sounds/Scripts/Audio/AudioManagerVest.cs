using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SavedVestAudio
{
    public string name;
    public AudioClip clip;
    public float time;
    public float volume;
    public float pitch;
    public bool loop;
    public float savedAtRealTime;
}

public class AudioManagerVest : MonoBehaviour
{
    public static AudioManagerVest Instance;

    public bool isHapticActive;
    public string currentlyPlayingSound = "";

    [Header("Global Vest Sounds")]
    public List<Sound> vestSounds;

    public bool isAllowedSound = true;

    [Header("Vest Settings")]
    [SerializeField] private AudioMixerGroup vestMixerGroup;

    private AudioSource vestSource;
    private List<Sound> activeLoops = new List<Sound>();
    private List<SavedVestAudio> audioStack = new List<SavedVestAudio>();
    private bool isPlayingOneShot = false;

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

        if (System.Environment.GetCommandLineArgs().Contains("-nohaptics"))
        {
            isHapticActive = false;
        }

        AudioManagerVest.Instance.isAllowedSound = isHapticActive;

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

    private void Start()
    {
        StartCoroutine(StateMonitorRoutine());
    }

    public void PlayVestSound(AudioClip clip, float volume = 1f)
    {
        if (clip == null || !isAllowedSound) return;

        SaveCurrentOneShotState();

        vestSource.clip = clip;
        vestSource.volume = volume;
        vestSource.pitch = 1f;
        vestSource.loop = false;
        vestSource.spatialBlend = 0f;
        vestSource.time = 0f;
        vestSource.Play();

        isPlayingOneShot = true;
        currentlyPlayingSound = "GenericClip";
    }

    public void PlayGlobalVestSound(string name)
    {
        if (!isAllowedSound) return;

        Sound s = vestSounds.Find(sound => sound.name == name);
        if (s == null) return;

        if (s.loop)
        {
            activeLoops.RemoveAll(x => x.name == name);
            activeLoops.Add(s);

            if (!isPlayingOneShot)
            {
                EvaluateVestState();
            }
        }
        else
        {
            SaveCurrentOneShotState();

            vestSource.clip = s.clip;
            vestSource.volume = s.volume;
            vestSource.pitch = s.pitch;
            vestSource.loop = false;
            vestSource.spatialBlend = 0f;
            vestSource.time = 0f;
            vestSource.Play();

            isPlayingOneShot = true;
            currentlyPlayingSound = s.name;
        }
    }

    public void StopGlobalVestSound(string name)
    {
        activeLoops.RemoveAll(x => x.name == name);
        audioStack.RemoveAll(x => x.name == name);

        if (currentlyPlayingSound == name)
        {
            vestSource.Stop();
            vestSource.clip = null;
            currentlyPlayingSound = "";
            isPlayingOneShot = false;
            EvaluateVestState();
        }
        else if (!isPlayingOneShot)
        {
            EvaluateVestState();
        }
    }

    private void SaveCurrentOneShotState()
    {
        if (isPlayingOneShot && vestSource.isPlaying && vestSource.clip != null)
        {
            if (vestSource.time >= vestSource.clip.length - 0.05f) return;

            SavedVestAudio saved = new SavedVestAudio();
            saved.name = currentlyPlayingSound;
            saved.clip = vestSource.clip;
            saved.time = vestSource.time;
            saved.volume = vestSource.volume;
            saved.pitch = vestSource.pitch;
            saved.loop = false;
            saved.savedAtRealTime = Time.time;

            audioStack.Add(saved);
        }
    }

    private void EvaluateVestState()
    {
        while (audioStack.Count > 0)
        {
            SavedVestAudio next = audioStack[audioStack.Count - 1];
            audioStack.RemoveAt(audioStack.Count - 1);

            if (next.clip != null)
            {
                float elapsedTimeSinceSave = Time.time - next.savedAtRealTime;
                float newPlaybackTime = next.time + elapsedTimeSinceSave;

                if (newPlaybackTime < next.clip.length)
                {
                    vestSource.clip = next.clip;
                    vestSource.volume = next.volume;
                    vestSource.pitch = next.pitch;
                    vestSource.loop = false;
                    vestSource.spatialBlend = 0f;
                    vestSource.time = newPlaybackTime;
                    vestSource.Play();

                    isPlayingOneShot = true;
                    currentlyPlayingSound = next.name;
                    return;
                }
            }
        }

        if (activeLoops.Count > 0)
        {
            Sound topLoop = activeLoops[activeLoops.Count - 1];

            if (currentlyPlayingSound != topLoop.name || !vestSource.isPlaying)
            {
                vestSource.clip = topLoop.clip;
                vestSource.volume = topLoop.volume;
                vestSource.pitch = topLoop.pitch;
                vestSource.loop = true;
                vestSource.spatialBlend = 0f;
                vestSource.time = 0f;
                vestSource.Play();

                currentlyPlayingSound = topLoop.name;
                isPlayingOneShot = false;
            }
        }
        else
        {
            vestSource.Stop();
            vestSource.clip = null;
            currentlyPlayingSound = "";
            isPlayingOneShot = false;
        }
    }

    public void ClearVestQueue()
    {
        audioStack.Clear();
        activeLoops.Clear();
        vestSource.Stop();
        vestSource.clip = null;
        currentlyPlayingSound = "";
        isPlayingOneShot = false;
    }

    private IEnumerator StateMonitorRoutine()
    {
        while (true)
        {
            if (isPlayingOneShot && !vestSource.isPlaying)
            {
                isPlayingOneShot = false;
                currentlyPlayingSound = "";
                EvaluateVestState();
            }

            if (!isPlayingOneShot && activeLoops.Count == 0 && vestSource.isPlaying)
            {
                vestSource.Stop();
                vestSource.clip = null;
                currentlyPlayingSound = "";
            }

            yield return null;
        }
    }
}