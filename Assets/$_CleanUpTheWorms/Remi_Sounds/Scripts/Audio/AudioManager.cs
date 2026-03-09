using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;

    [Range(0f, 1f)]
    public float spatialBlend = 0f;

    public bool loop;
    public bool playOnAwake;
    public bool preventOverlay;

    public AudioMixerGroup mixerGroup;

    [HideInInspector]
    public AudioSource source;
}

[System.Serializable]
public class AmbientSoundSettings
{
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
    [Range(0f, 1f)] public float targetVolume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public float fadeInDuration = 2f;

    public bool reduceVolumeInsteadOfStop;
    [Range(0f, 1f)] public float reducedVolume = 0.2f;

    [HideInInspector] public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Global Sounds")]
    public List<Sound> sounds;

    [Header("3D Sounds Prefabs")]
    [SerializeField] private GameObject soundFXPrefab;

    [Header("Spawner Parent")]
    [SerializeField] private Transform soundSpawnerParent;

    [Header("Ambient Event Settings")]
    [SerializeField] private bool startAmbientOnStart = true;
    [SerializeField] private List<AmbientSoundSettings> ambientSounds;

    [SerializeField] private AudioClip lightOffClip;
    [SerializeField] private string vestLightOffSoundName;
    [SerializeField, Range(0f, 1f)] private float lightOffVolume = 1f;

    [SerializeField] public float lightOffWaitTime = 3f;

    [SerializeField] private AudioClip generatorClip;
    [SerializeField] private string vestGeneratorSoundName;
    [SerializeField, Range(0f, 1f)] private float generatorVolume = 1f;

    [SerializeField] public float generatorToLightWaitTime = 2f;

    [SerializeField] private AudioClip lightOnClip;
    [SerializeField] private string vestLightOnSoundName;
    [SerializeField, Range(0f, 1f)] private float lightOnVolume = 1f;

    [Header("Progressive Light Settings")]
    [SerializeField] private List<GameObject> lightGroups;
    [SerializeField] private float delayBetweenLights = 0.5f;

    [Header("Endgame Settings")]
    [SerializeField] private GameObject crowdLarvaObject;
    [SerializeField] private List<GameObject> objectsToDisableOnSecondBlackout;
    [SerializeField] private List<GameObject> objectsToEnableOnSecondBlackout;

    [Header("Lightmap Settings")]
    [SerializeField] private Texture2D[] darkLightmapColors;
    [SerializeField] private Texture2D[] litLightmapColors;

    [Header("Random 3D Ambient Sound")]
    [SerializeField] private AudioClip random3DClip;
    [SerializeField, Range(0f, 1f)] private float random3DVolume = 1f;
    [SerializeField] private AudioMixerGroup random3DMixerGroup;
    [SerializeField] private float minRandomInterval = 5f;
    [SerializeField] private float maxRandomInterval = 15f;
    [SerializeField] private float spawnRadius = 10f;

    private Dictionary<AudioSource, Coroutine> activeFades = new Dictionary<AudioSource, Coroutine>();
    private Coroutine ambientRoutine;
    private Coroutine random3DRoutine;
    private bool ambientActive = false;
    private int blackoutCount = 0;

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

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.spatialBlend = s.spatialBlend;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixerGroup;
            s.source.playOnAwake = s.playOnAwake;

            if (s.playOnAwake) s.source.Play();
        }

        foreach (AmbientSoundSettings amb in ambientSounds)
        {
            amb.source = gameObject.AddComponent<AudioSource>();
            amb.source.clip = amb.clip;
            amb.source.loop = true;
            amb.source.outputAudioMixerGroup = amb.mixerGroup;
            amb.source.pitch = amb.pitch;
            amb.source.volume = 0f;
            amb.source.playOnAwake = false;
        }
    }

    private void Start()
    {
        if (lightGroups != null && lightGroups.Count > 0)
        {
            for (int i = 0; i < lightGroups.Count; i++)
            {
                if (lightGroups[i] != null)
                {
                    lightGroups[i].SetActive(i == 0);
                }
            }
        }

        if (startAmbientOnStart)
        {
            StartAmbientSequence();
        }
    }

    public void Play(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null) return;

        if (s.preventOverlay || s.loop)
        {
            s.source.Play();
        }
        else
        {
            s.source.PlayOneShot(s.clip);
        }
    }

    public void Stop(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s != null) s.source.Stop();
    }

    public void PlayClipAtPoint(AudioClip clip, Vector3 position, AudioMixerGroup group, float volume = 1f, float spatialBlend = 1f)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio" + " - " + clip.name);
        tempGO.transform.parent = soundSpawnerParent;
        tempGO.transform.position = position;

        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = group;
        audioSource.volume = volume;
        audioSource.spatialBlend = spatialBlend;

        audioSource.minDistance = 1f;
        audioSource.maxDistance = 20f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        audioSource.Play();

        Destroy(tempGO, clip.length);
    }

    public AudioSource PrepareSound(AudioClip clip, Vector3 position, AudioMixerGroup group, Transform parent = null, float volume = 1f, float spatialBlend = 1f, bool loop = false)
    {
        if (clip == null) return null;

        GameObject tempGO = new GameObject("PreparedAudio" + " - " + clip.name);
        tempGO.transform.parent = soundSpawnerParent;
        tempGO.transform.position = position;
        if (parent != null) tempGO.transform.parent = parent;

        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = group;
        audioSource.volume = volume;
        audioSource.spatialBlend = spatialBlend;
        audioSource.loop = loop;

        audioSource.minDistance = 1f;
        audioSource.maxDistance = 20f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.playOnAwake = false;

        return audioSource;
    }

    public void PlayPreparedSound(AudioSource source, Vector3 pos, bool canDestroy = false)
    {
        if (source == null) return;
        if (!source.gameObject.activeInHierarchy) return;

        source.transform.position = pos;
        source.Play();

        if (!canDestroy) return;
        Destroy(source.gameObject, source.clip.length);
    }

    public void StopPreparedSound(AudioSource source)
    {
        if (source == null) return;
        source.Stop();
    }

    public void PlayPreparedSoundProgressive(AudioSource source, float targetVolume, float duration)
    {
        if (source == null) return;

        if (activeFades.ContainsKey(source) && activeFades[source] != null)
        {
            StopCoroutine(activeFades[source]);
            activeFades.Remove(source);
        }

        if (!source.isPlaying)
        {
            source.volume = 0f;
            source.Play();
        }

        Coroutine c = StartCoroutine(FadeSoundRoutine(source, targetVolume, duration, false));
        activeFades[source] = c;
    }

    public void StopPreparedSoundProgressive(AudioSource source, float duration)
    {
        if (source == null || !source.gameObject.activeInHierarchy) return;

        if (activeFades.ContainsKey(source) && activeFades[source] != null)
        {
            StopCoroutine(activeFades[source]);
            activeFades.Remove(source);
        }

        Coroutine c = StartCoroutine(FadeSoundRoutine(source, 0f, duration, true));
        activeFades[source] = c;
    }

    private IEnumerator FadeSoundRoutine(AudioSource source, float targetVolume, float duration, bool stopOnComplete)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (source == null) yield break;

            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        if (source != null)
        {
            source.volume = targetVolume;
            if (stopOnComplete)
            {
                source.Stop();
            }
            if (activeFades.ContainsKey(source))
            {
                activeFades.Remove(source);
            }
        }
    }

    [ContextMenu("Start Ambient Sequence")]
    public void StartAmbientSequence()
    {
        ambientActive = true;
        foreach (AmbientSoundSettings amb in ambientSounds)
        {
            if (!amb.source.isPlaying)
            {
                amb.source.Play();
            }
            PlayPreparedSoundProgressive(amb.source, amb.targetVolume, amb.fadeInDuration);
        }

        if (random3DRoutine != null) StopCoroutine(random3DRoutine);
        random3DRoutine = StartCoroutine(Random3DAmbientRoutine());
    }

    [ContextMenu("Trigger Light Off Event")]
    public void TriggerLightOffEvent()
    {
        blackoutCount++;

        if (blackoutCount == 2)
        {
            if (objectsToDisableOnSecondBlackout != null)
            {
                foreach (GameObject obj in objectsToDisableOnSecondBlackout)
                {
                    if (obj != null) obj.SetActive(false);
                }
            }

            if (objectsToEnableOnSecondBlackout != null)
            {
                foreach (GameObject obj in objectsToEnableOnSecondBlackout)
                {
                    if (obj != null) obj.SetActive(true);
                }
            }
        }

        ambientActive = false;
        if (random3DRoutine != null) StopCoroutine(random3DRoutine);

        if (lightGroups != null)
        {
            foreach (GameObject group in lightGroups)
            {
                if (group != null) group.SetActive(false);
            }
        }

        SetLightmaps(darkLightmapColors);

        if (lightOffClip != null)
        {
            PlayClipAtPoint(lightOffClip, transform.position, ambientSounds.Count > 0 ? ambientSounds[0].mixerGroup : null, lightOffVolume);
        }

        if (!string.IsNullOrEmpty(vestLightOffSoundName) && AudioManagerVest.Instance != null)
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(vestLightOffSoundName);
        }

        foreach (AmbientSoundSettings amb in ambientSounds)
        {
            if (activeFades.ContainsKey(amb.source) && activeFades[amb.source] != null)
            {
                StopCoroutine(activeFades[amb.source]);
                activeFades.Remove(amb.source);
            }

            if (amb.reduceVolumeInsteadOfStop)
            {
                amb.source.volume = amb.reducedVolume;
            }
            else
            {
                amb.source.Stop();
                amb.source.volume = 0f;
            }
        }

        if (ambientRoutine != null) StopCoroutine(ambientRoutine);
        ambientRoutine = StartCoroutine(LightOffSequenceRoutine());
    }

    private IEnumerator LightOffSequenceRoutine()
    {
        yield return new WaitForSeconds(lightOffWaitTime);

        if (generatorClip != null)
        {
            PlayClipAtPoint(generatorClip, transform.position, ambientSounds.Count > 0 ? ambientSounds[0].mixerGroup : null, generatorVolume);
        }

        if (!string.IsNullOrEmpty(vestGeneratorSoundName) && AudioManagerVest.Instance != null)
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(vestGeneratorSoundName);
        }

        yield return new WaitForSeconds(generatorToLightWaitTime);

        int lightsToReveal = 1;
        if (lightGroups != null)
        {
            lightsToReveal = Mathf.Min(blackoutCount + 1, lightGroups.Count);
        }

        SetLightmaps(litLightmapColors);

        for (int i = 0; i < lightsToReveal; i++)
        {
            if (lightGroups != null && lightGroups.Count > i && lightGroups[i] != null)
            {
                lightGroups[i].SetActive(true);
            }

            if (lightOnClip != null)
            {
                PlayClipAtPoint(lightOnClip, transform.position, ambientSounds.Count > 0 ? ambientSounds[0].mixerGroup : null, lightOnVolume);
            }

            if (!string.IsNullOrEmpty(vestLightOnSoundName) && AudioManagerVest.Instance != null)
            {
                AudioManagerVest.Instance.PlayGlobalVestSound(vestLightOnSoundName);
            }

            if (i == lightGroups.Count - 1 && crowdLarvaObject != null)
            {
                crowdLarvaObject.SetActive(true);
            }

            if (i < lightsToReveal - 1)
            {
                yield return new WaitForSeconds(delayBetweenLights);
            }
        }

        StartAmbientSequence();
    }

    private IEnumerator Random3DAmbientRoutine()
    {
        while (ambientActive)
        {
            float waitTime = UnityEngine.Random.Range(minRandomInterval, maxRandomInterval);
            yield return new WaitForSeconds(waitTime);

            if (Camera.main != null && random3DClip != null && soundFXPrefab != null)
            {
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;
                Vector3 spawnPos = Camera.main.transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

                GameObject fxGo = Instantiate(soundFXPrefab, spawnPos, Quaternion.identity, soundSpawnerParent);
                AudioSource fxSource = fxGo.GetComponent<AudioSource>();
                if (fxSource == null) fxSource = fxGo.AddComponent<AudioSource>();

                fxSource.clip = random3DClip;
                fxSource.outputAudioMixerGroup = random3DMixerGroup;
                fxSource.spatialBlend = 1f;
                fxSource.volume = random3DVolume;
                fxSource.Play();

                Destroy(fxGo, random3DClip.length);
            }
        }
    }

    private void SetLightmaps(Texture2D[] colorMaps)
    {
        if (colorMaps == null || colorMaps.Length == 0) return;

        LightmapData[] lightmapDataArray = new LightmapData[colorMaps.Length];
        for (int i = 0; i < colorMaps.Length; i++)
        {
            lightmapDataArray[i] = new LightmapData();
            lightmapDataArray[i].lightmapColor = colorMaps[i];
        }
        LightmapSettings.lightmaps = lightmapDataArray;
    }
}