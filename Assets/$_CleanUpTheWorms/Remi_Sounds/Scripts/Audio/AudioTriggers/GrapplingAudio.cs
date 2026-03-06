using UnityEngine;
using System.Collections;

public class GrapplingAudio : MonoBehaviour
{
    [Header("Telescopie Settings")]
    [SerializeField] private Sound telescopieAudio;
    [SerializeField] private string telescopieVestName;
    [SerializeField] private float telescopieFadeInDuration = 0.5f;
    [SerializeField] private float telescopieFadeOutDuration = 0.5f;

    [Header("Open/Close Graplin Settings")]
    [SerializeField] private Sound graplinOpenCloseAudio;
    [SerializeField] private string graplinOpenCloseVestName;
    [SerializeField] private float graplinFadeInDuration = 0.5f;
    [SerializeField] private float graplinFadeOutDuration = 0.5f;

    [Header("Claw Grab Settings")]
    [SerializeField] private Sound clawGrabAudio;
    [SerializeField] private string clawGrabVestName;

    private Coroutine telescopieFade;
    private Coroutine graplinFade;

    private void Awake()
    {
        InitializeSound(telescopieAudio, "TelescopieAudioSource");
        InitializeSound(graplinOpenCloseAudio, "GraplinOpenCloseAudioSource");
        InitializeSound(clawGrabAudio, "ClawGrabAudioSource");
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
            s.source.volume = 0f;
            s.source.Play();
            StartCoroutine(FadeIn(s.source, s.volume, 0.5f));
        }
    }

    [ContextMenu("Start Telescopie Loop")]
    public void StartTelescopie()
    {
        if (telescopieAudio != null && telescopieAudio.source != null)
        {
            if (telescopieFade != null)
            {
                StopCoroutine(telescopieFade);
                telescopieFade = null;
            }

            if (!telescopieAudio.source.isPlaying)
            {
                telescopieAudio.source.volume = 0f;

                if (telescopieAudio.preventOverlay || telescopieAudio.loop)
                {
                    telescopieAudio.source.Play();
                }
                else
                {
                    telescopieAudio.source.PlayOneShot(telescopieAudio.clip);
                }
            }

            telescopieFade = StartCoroutine(FadeIn(telescopieAudio.source, telescopieAudio.volume, telescopieFadeInDuration));
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(telescopieVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(telescopieVestName);
        }
    }

    [ContextMenu("Stop Telescopie Loop")]
    public void StopTelescopie()
    {
        if (telescopieAudio != null && telescopieAudio.source != null && telescopieAudio.source.isPlaying)
        {
            if (telescopieFade != null) StopCoroutine(telescopieFade);
            telescopieFade = StartCoroutine(FadeOut(telescopieAudio.source, telescopieAudio.volume, telescopieFadeOutDuration));
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(telescopieVestName))
        {
            AudioManagerVest.Instance.StopGlobalVestSound(telescopieVestName);
        }
    }

    [ContextMenu("Start Open/Close Graplin Loop")]
    public void StartOpenCloseGraplin()
    {
        if (graplinOpenCloseAudio != null && graplinOpenCloseAudio.source != null)
        {
            if (graplinFade != null)
            {
                StopCoroutine(graplinFade);
                graplinFade = null;
            }

            if (!graplinOpenCloseAudio.source.isPlaying)
            {
                graplinOpenCloseAudio.source.volume = 0f;

                if (graplinOpenCloseAudio.preventOverlay || graplinOpenCloseAudio.loop)
                {
                    graplinOpenCloseAudio.source.Play();
                }
                else
                {
                    graplinOpenCloseAudio.source.PlayOneShot(graplinOpenCloseAudio.clip);
                }
            }

            graplinFade = StartCoroutine(FadeIn(graplinOpenCloseAudio.source, graplinOpenCloseAudio.volume, graplinFadeInDuration));
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(graplinOpenCloseVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(graplinOpenCloseVestName);
        }
    }

    [ContextMenu("Stop Open/Close Graplin Loop")]
    public void StopOpenCloseGraplin()
    {
        if (graplinOpenCloseAudio != null && graplinOpenCloseAudio.source != null && graplinOpenCloseAudio.source.isPlaying)
        {
            if (graplinFade != null) StopCoroutine(graplinFade);
            graplinFade = StartCoroutine(FadeOut(graplinOpenCloseAudio.source, graplinOpenCloseAudio.volume, graplinFadeOutDuration));
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(graplinOpenCloseVestName))
        {
            AudioManagerVest.Instance.StopGlobalVestSound(graplinOpenCloseVestName);
        }
    }

    [ContextMenu("Play Claw Grab (One-Shot)")]
    public void PlayClawGrab()
    {
        if (clawGrabAudio != null && clawGrabAudio.source != null)
        {
            if (clawGrabAudio.preventOverlay || clawGrabAudio.loop)
            {
                clawGrabAudio.source.Play();
            }
            else
            {
                clawGrabAudio.source.PlayOneShot(clawGrabAudio.clip);
            }
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(clawGrabVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(clawGrabVestName);
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