using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR;

public class RobotAudio : MonoBehaviour
{
    public static RobotAudio Instance;

    [Header("Upgrade Settings")]
    [SerializeField] private Sound upgradeAudio;
    [SerializeField] private string upgradeVestName;

    public VisualEffect vfx;
    public float duration = 2.0f;
    private bool hasPlayed = false;


    [Header("Speech Settings")]
    [SerializeField] private Sound[] speechAudios;
    [SerializeField] private float minSpeechInterval = 5f;
    [SerializeField] private float maxSpeechInterval = 15f;

    [Header("Surprised Settings")]
    [SerializeField] private Sound surprisedAudio;

    [Header("Validation Settings")]
    [SerializeField] private Sound validationAudio;

    [Header("Wheel Loop Settings")]
    [SerializeField] private Sound wheelAudio;
    [SerializeField] private float wheelFadeInDuration = 0.5f;
    [SerializeField] private float wheelFadeOutDuration = 0.5f;

    private Coroutine wheelFade;
    private List<int> recentSpeechIndices = new List<int>();

    public XRNode controllerNode = XRNode.RightHand;
    public XRNode controllerNodeLeft = XRNode.LeftHand;

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

        InitializeSound(upgradeAudio, "UpgradeAudioSource");

        vfx.Stop();

        if (speechAudios != null)
        {
            for (int i = 0; i < speechAudios.Length; i++)
            {
                InitializeSound(speechAudios[i], "SpeechAudioSource_" + i);
            }
        }

        InitializeSound(surprisedAudio, "SurprisedAudioSource");
        InitializeSound(validationAudio, "ValidationAudioSource");
        InitializeSound(wheelAudio, "WheelAudioSource");
    }

    private void Start()
    {
        StartCoroutine(MonitorRobotState());
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
            if (s == wheelAudio)
            {
                s.source.volume = 0f;
                s.source.Play();
                wheelFade = StartCoroutine(FadeIn(s.source, s.volume, wheelFadeInDuration));
            }
            else
            {
                s.source.Play();
            }
        }
    }

    private IEnumerator MonitorRobotState()
    {
        yield return new WaitUntil(() => S_StateRobot.Instance != null);

        RobotState lastState = S_StateRobot.Instance.currentState;

        if (lastState == RobotState.WALK)
        {
            StartWheel();
        }

        while (true)
        {
            RobotState currentState = S_StateRobot.Instance.currentState;

            if (currentState != lastState)
            {
                if (currentState == RobotState.WALK)
                {
                    StartWheel();
                }
                else if (lastState == RobotState.WALK)
                {
                    StopWheel();
                }

                if (currentState == RobotState.CLUE)
                {
                    PlaySurprised();
                }

                lastState = currentState;
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

    [ContextMenu("Play Upgrade Event")]
    public void PlayUpgrade()
    {
        if (upgradeAudio != null && upgradeAudio.source != null)
        {
            if (upgradeAudio.preventOverlay || upgradeAudio.loop)
            {
                upgradeAudio.source.Play();
            }
            else
            {
                upgradeAudio.source.PlayOneShot(upgradeAudio.clip);
            }
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(upgradeVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(upgradeVestName);
        }

        InputDevices.GetDeviceAtXRNode(controllerNode).SendHapticImpulse(0u, 1f, 0.4f);
        InputDevices.GetDeviceAtXRNode(controllerNodeLeft).SendHapticImpulse(0u, 1f, 0.4f);

        TriggerVFX();
    }

    public void TriggerVFX()
    {
        if (vfx != null && !hasPlayed)
        {
            hasPlayed = true;
            vfx.Play();
            StartCoroutine(StopVFXRoutine());
        }
    }

    private IEnumerator StopVFXRoutine()
    {
        yield return new WaitForSeconds(duration);
        vfx.Stop();
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

    [ContextMenu("Play Surprised Event")]
    public void PlaySurprised()
    {
        if (surprisedAudio != null && surprisedAudio.source != null)
        {
            if (surprisedAudio.preventOverlay || surprisedAudio.loop)
            {
                surprisedAudio.source.Play();
            }
            else
            {
                surprisedAudio.source.PlayOneShot(surprisedAudio.clip);
            }
        }
    }

    [ContextMenu("Play Validation Event")]
    public void PlayValidation()
    {
        if (validationAudio != null && validationAudio.source != null)
        {
            if (validationAudio.preventOverlay || validationAudio.loop)
            {
                validationAudio.source.Play();
            }
            else
            {
                validationAudio.source.PlayOneShot(validationAudio.clip);
            }
        }
    }

    [ContextMenu("Start Wheel Loop")]
    public void StartWheel()
    {
        if (wheelAudio != null && wheelAudio.source != null)
        {
            if (wheelFade != null)
            {
                StopCoroutine(wheelFade);
                wheelFade = null;
            }

            if (!wheelAudio.source.isPlaying)
            {
                wheelAudio.source.volume = 0f;

                if (wheelAudio.preventOverlay || wheelAudio.loop)
                {
                    wheelAudio.source.Play();
                }
                else
                {
                    wheelAudio.source.PlayOneShot(wheelAudio.clip);
                }
            }

            wheelFade = StartCoroutine(FadeIn(wheelAudio.source, wheelAudio.volume, wheelFadeInDuration));
        }
    }

    [ContextMenu("Stop Wheel Loop")]
    public void StopWheel()
    {
        if (wheelAudio != null && wheelAudio.source != null && wheelAudio.source.isPlaying)
        {
            if (wheelFade != null) StopCoroutine(wheelFade);
            wheelFade = StartCoroutine(FadeOut(wheelAudio.source, wheelAudio.volume, wheelFadeOutDuration));
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