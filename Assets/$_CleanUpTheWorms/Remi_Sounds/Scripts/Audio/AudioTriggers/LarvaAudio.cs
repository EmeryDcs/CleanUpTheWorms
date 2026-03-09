using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class LarvaAudio : MonoBehaviour
{
    [Header("Speech Settings")]
    [SerializeField] private Sound[] speechAudios;
    [SerializeField] private float minSpeechInterval = 5f;
    [SerializeField] private float maxSpeechInterval = 15f;

    [Header("Movement Loop Settings")]
    [SerializeField] private Sound movementAudio;
    [SerializeField] private float movementFadeInDuration = 0.5f;
    [SerializeField] private float movementFadeOutDuration = 0.5f;

    private Coroutine movementFade;
    private List<int> recentSpeechIndices = new List<int>();

    private void Awake()
    {
        if (speechAudios != null)
        {
            for (int i = 0; i < speechAudios.Length; i++)
            {
                InitializeSound(speechAudios[i], "SpeechAudioSource_" + i);
            }
        }

        InitializeSound(movementAudio, "MovementAudioSource");
    }

    private void Start()
    {
        StartCoroutine(MonitorLarvaState());
        StartCoroutine(RandomSpeechRoutine());
    }

    private void InitializeSound(Sound s, string childName)
    {
        if (s == null || s.clip == null) return;

        GameObject childObj = new GameObject(childName);
        childObj.transform.SetParent(transform);
        childObj.transform.localPosition = Vector3.zero;

        s.source = childObj.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.spatialBlend = s.spatialBlend;
        s.source.outputAudioMixerGroup = s.mixerGroup;
        s.source.loop = s.loop;
        s.source.playOnAwake = s.playOnAwake;

        if (s.playOnAwake)
        {
            if (s == movementAudio)
            {
                s.source.volume = 0f;
                s.source.Play();
                movementFade = StartCoroutine(FadeIn(s.source, s.volume, movementFadeInDuration));
            }
            else
            {
                s.source.Play();
            }
        }
    }

    private IEnumerator MonitorLarvaState()
    {
        NavMeshAgent agent = GetComponentInParent<NavMeshAgent>();
        yield return new WaitUntil(() => agent != null);

        bool wasMoving = false;

        while (true)
        {
            bool isMoving = agent.enabled && agent.velocity.sqrMagnitude > 0.01f;

            if (isMoving && !wasMoving)
            {
                StartMovement();
                wasMoving = true;
            }
            else if (!isMoving && wasMoving)
            {
                StopMovement();
                wasMoving = false;
            }

            yield return null;
        }
    }

    private IEnumerator RandomSpeechRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpeechInterval, maxSpeechInterval);
            yield return new WaitForSeconds(waitTime);
            PlayRandomSpeech();
        }
    }

    [ContextMenu("Play Random Speech")]
    public void PlayRandomSpeech()
    {
        if (speechAudios != null && speechAudios.Length > 0)
        {
            int maxHistory = Mathf.Min(3, speechAudios.Length - 1);
            int randomIndex = Random.Range(0, speechAudios.Length);

            if (maxHistory > 0)
            {
                int attempts = 0;
                while (recentSpeechIndices.Contains(randomIndex) && attempts < 100)
                {
                    randomIndex = Random.Range(0, speechAudios.Length);
                    attempts++;
                }

                recentSpeechIndices.Add(randomIndex);
                if (recentSpeechIndices.Count > maxHistory)
                {
                    recentSpeechIndices.RemoveAt(0);
                }
            }

            Sound s = speechAudios[randomIndex];

            if (s != null && s.source != null)
            {
                if (s.preventOverlay || s.loop)
                {
                    s.source.Play();
                }
                else
                {
                    s.source.PlayOneShot(s.clip);
                }
            }
        }
    }

    [ContextMenu("Start Movement Loop")]
    public void StartMovement()
    {
        if (movementAudio != null && movementAudio.source != null)
        {
            if (movementFade != null)
            {
                StopCoroutine(movementFade);
                movementFade = null;
            }

            if (!movementAudio.source.isPlaying)
            {
                movementAudio.source.volume = 0f;

                if (movementAudio.preventOverlay || movementAudio.loop)
                {
                    movementAudio.source.Play();
                }
                else
                {
                    movementAudio.source.PlayOneShot(movementAudio.clip);
                }
            }

            movementFade = StartCoroutine(FadeIn(movementAudio.source, movementAudio.volume, movementFadeInDuration));
        }
    }

    [ContextMenu("Stop Movement Loop")]
    public void StopMovement()
    {
        if (movementAudio != null && movementAudio.source != null && movementAudio.source.isPlaying)
        {
            if (movementFade != null) StopCoroutine(movementFade);
            movementFade = StartCoroutine(FadeOut(movementAudio.source, movementAudio.volume, movementFadeOutDuration));
        }
    }

    private IEnumerator FadeIn(AudioSource source, float targetVolume, float duration)
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
        }
    }

    private IEnumerator FadeOut(AudioSource source, float originalVolume, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (source == null) yield break;

            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        if (source != null)
        {
            source.Stop();
            source.volume = originalVolume;
        }
    }
}