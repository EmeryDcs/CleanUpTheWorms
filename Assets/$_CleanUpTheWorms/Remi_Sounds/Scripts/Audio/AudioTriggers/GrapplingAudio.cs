using UnityEngine;
using System.Collections;

public class GrapplingAudio : MonoBehaviour
{
    [Header("Telescopie Settings")]
    [SerializeField] private Sound telescopieAudio;
    [SerializeField] private string telescopieVestName;
    [SerializeField] private float telescopieFadeInDuration = 0.5f;
    [SerializeField] private float telescopieFadeOutDuration = 0.5f;
    [SerializeField] private GameObject tigeReference;

    [Header("Open/Close Graplin Settings")]
    [SerializeField] private Sound graplinOpenCloseAudio;
    [SerializeField] private string graplinOpenCloseVestName;
    [SerializeField] private float graplinFadeInDuration = 0.5f;
    [SerializeField] private float graplinFadeOutDuration = 0.5f;

    [Header("Claw Grab Settings")]
    [SerializeField] private Sound clawGrabAudio;
    [SerializeField] private string clawGrabVestName;

    [Header("Holding Settings")]
    [SerializeField] private Sound holdingAudio;
    [SerializeField] private string holdingVestName;
    [SerializeField] private float holdingFadeInDuration = 0.5f;
    [SerializeField] private float holdingFadeOutDuration = 0.5f;

    private Coroutine telescopieFade;
    private Coroutine graplinFade;
    private Coroutine holdingFade;

    private PlayerInputSystem inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputSystem();
        inputActions.Player.Enable();

        InitializeSound(telescopieAudio, "TelescopieAudioSource");
        InitializeSound(graplinOpenCloseAudio, "GraplinOpenCloseAudioSource");
        InitializeSound(clawGrabAudio, "ClawGrabAudioSource");
        InitializeSound(holdingAudio, "HoldingAudioSource");
    }

    private void Start()
    {
        if (tigeReference != null)
        {
            StartCoroutine(MonitorTelescopieState());
        }
        StartCoroutine(MonitorGrappleState());
    }

    private void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Player.Disable();
        }
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

    private IEnumerator MonitorTelescopieState()
    {
        Vector3 lastScale = tigeReference.transform.localScale;
        bool isMoving = false;

        while (true)
        {
            Vector3 currentScale = tigeReference.transform.localScale;
            bool currentlyMoving = Vector3.Distance(currentScale, lastScale) > 0.001f;

            if (currentlyMoving && !isMoving)
            {
                StartTelescopie();
                isMoving = true;
            }
            else if (!currentlyMoving && isMoving)
            {
                StopTelescopie();
                isMoving = false;
            }

            lastScale = currentScale;
            yield return null;
        }
    }

    private IEnumerator MonitorGrappleState()
    {
        yield return new WaitUntil(() => grab.Instance != null);

        bool wasGraplinOpen = false;
        bool wasHolding = false;

        while (true)
        {
            float triggerValue = inputActions.Player.Grab.ReadValue<float>();
            bool isGraplinOpen = triggerValue > 0f;

            if (isGraplinOpen && !wasGraplinOpen)
            {
                StartOpenCloseGraplin();
            }
            else if (!isGraplinOpen && wasGraplinOpen)
            {
                StopOpenCloseGraplin();
            }

            wasGraplinOpen = isGraplinOpen;

            GameObject currentGrabbedElm = grab.Instance.GetGrabbedElm();
            bool isHolding = currentGrabbedElm != null;

            if (isHolding && !wasHolding)
            {
                PlayClawGrab();
                StartHolding();
            }
            else if (!isHolding && wasHolding)
            {
                StopHolding();
            }
            wasHolding = isHolding;

            yield return null;
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
        if (telescopieAudio != null && telescopieAudio.source != null)
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
        if (graplinOpenCloseAudio != null && graplinOpenCloseAudio.source != null)
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

    [ContextMenu("Start Holding Loop")]
    public void StartHolding()
    {
        if (holdingAudio != null && holdingAudio.source != null)
        {
            if (holdingFade != null)
            {
                StopCoroutine(holdingFade);
                holdingFade = null;
            }

            if (!holdingAudio.source.isPlaying)
            {
                holdingAudio.source.volume = 0f;

                if (holdingAudio.preventOverlay || holdingAudio.loop)
                {
                    holdingAudio.source.Play();
                }
                else
                {
                    holdingAudio.source.PlayOneShot(holdingAudio.clip);
                }
            }

            holdingFade = StartCoroutine(FadeIn(holdingAudio.source, holdingAudio.volume, holdingFadeInDuration));
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(holdingVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(holdingVestName);
        }
    }

    [ContextMenu("Stop Holding Loop")]
    public void StopHolding()
    {
        if (holdingAudio != null && holdingAudio.source != null)
        {
            if (holdingFade != null) StopCoroutine(holdingFade);
            holdingFade = StartCoroutine(FadeOut(holdingAudio.source, holdingAudio.volume, holdingFadeOutDuration));
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(holdingVestName))
        {
            AudioManagerVest.Instance.StopGlobalVestSound(holdingVestName);
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