using UnityEngine;
using System.Collections;

public class SpecialEventsAudio : MonoBehaviour
{
    [Header("Crowd Larva Intro Settings")]
    [SerializeField] private Sound crowdLarvaIntroAudio;
    [SerializeField] private string crowdLarvaIntroVestName;
    [SerializeField] private float crowdLarvaIntroWaitTime = 2f;

    [Header("Crowd Larva Main Settings")]
    [SerializeField] private Sound crowdLarvaAudio;
    [SerializeField] private string crowdLarvaVestName;
    [SerializeField] private float crowdLarvaFadeInDuration = 1f;

    [Header("Break Glass Settings")]
    [SerializeField] private Sound breakGlassAudio;
    [SerializeField] private string breakGlassVestName;

    private Coroutine crowdLarvaSequenceRoutine;
    private Coroutine crowdLarvaFade;

    private void Awake()
    {
        InitializeSound(crowdLarvaIntroAudio, "CrowdLarvaIntroAudioSource");
        InitializeSound(crowdLarvaAudio, "CrowdLarvaAudioSource");
        InitializeSound(breakGlassAudio, "BreakGlassAudioSource");
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
            s.source.Play();
        }
    }

    [ContextMenu("Play Crowd Larva Event")]
    public void PlayCrowdLarva()
    {
        if (crowdLarvaSequenceRoutine != null)
        {
            StopCoroutine(crowdLarvaSequenceRoutine);
        }
        crowdLarvaSequenceRoutine = StartCoroutine(CrowdLarvaSequence());
    }

    private IEnumerator CrowdLarvaSequence()
    {
        if (crowdLarvaIntroAudio != null && crowdLarvaIntroAudio.source != null)
        {
            if (crowdLarvaIntroAudio.preventOverlay || crowdLarvaIntroAudio.loop)
            {
                crowdLarvaIntroAudio.source.Play();
            }
            else
            {
                crowdLarvaIntroAudio.source.PlayOneShot(crowdLarvaIntroAudio.clip);
            }
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(crowdLarvaIntroVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(crowdLarvaIntroVestName);
        }

        yield return new WaitForSeconds(crowdLarvaIntroWaitTime);

        if (crowdLarvaAudio != null && crowdLarvaAudio.source != null)
        {
            if (crowdLarvaFade != null)
            {
                StopCoroutine(crowdLarvaFade);
                crowdLarvaFade = null;
            }

            if (!crowdLarvaAudio.source.isPlaying)
            {
                crowdLarvaAudio.source.volume = 0f;

                if (crowdLarvaAudio.preventOverlay || crowdLarvaAudio.loop)
                {
                    crowdLarvaAudio.source.Play();
                }
                else
                {
                    crowdLarvaAudio.source.PlayOneShot(crowdLarvaAudio.clip);
                }
            }

            crowdLarvaFade = StartCoroutine(FadeIn(crowdLarvaAudio.source, crowdLarvaAudio.volume, crowdLarvaFadeInDuration));
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(crowdLarvaVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(crowdLarvaVestName);
        }
    }

    [ContextMenu("Play Break Glass Event")]
    public void PlayBreakGlass()
    {
        if (breakGlassAudio != null && breakGlassAudio.source != null)
        {
            if (breakGlassAudio.preventOverlay || breakGlassAudio.loop)
            {
                breakGlassAudio.source.Play();
            }
            else
            {
                breakGlassAudio.source.PlayOneShot(breakGlassAudio.clip);
            }
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(breakGlassVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(breakGlassVestName);
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
}