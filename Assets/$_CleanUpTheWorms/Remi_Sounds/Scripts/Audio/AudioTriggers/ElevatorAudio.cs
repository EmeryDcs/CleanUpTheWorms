using UnityEngine;
using System.Collections;

public class ElevatorAudio : MonoBehaviour
{
    [Header("Latency Compensation")]
    [SerializeField] private double vestLatencyDelay = 0.15;

    [Header("Elevator Going Up Settings")]
    [SerializeField] private Sound elevatorGoingUpAudio;
    [SerializeField] private string elevatorGoingUpVestName;

    [Header("Elevator Door Open Settings")]
    [SerializeField] private Sound elevatorDoorOpenAudio;
    [SerializeField] private string elevatorDoorOpenVestName;

    private Coroutine elevatorSequence;

    private void Awake()
    {
        InitializeSound(elevatorGoingUpAudio, "ElevatorGoingUpAudioSource");
        InitializeSound(elevatorDoorOpenAudio, "ElevatorDoorOpenAudioSource");
    }

    private void Start()
    {
        PlayElevatorGoingUp();
        StartCoroutine(WaitForTutorialStateRoutine());
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

    private IEnumerator WaitForTutorialStateRoutine()
    {
        yield return new WaitUntil(() => StateMachineGame.Instance != null && StateMachineGame.Instance.state == GameState.TUTORIAL);
        S_Elevator.Instance.OpenElevator();
        PlayElevatorDoorOpen();
    }

    [ContextMenu("Play Elevator Going Up Event")]
    public void PlayElevatorGoingUp()
    {
        if (elevatorSequence != null)
        {
            StopCoroutine(elevatorSequence);
        }
        elevatorSequence = StartCoroutine(ElevatorSequenceRoutine());
    }

    private IEnumerator ElevatorSequenceRoutine()
    {
        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(elevatorGoingUpVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(elevatorGoingUpVestName);
        }

        if (elevatorGoingUpAudio != null && elevatorGoingUpAudio.source != null)
        {
            if (vestLatencyDelay > 0)
            {
                double exactPlayTime = AudioSettings.dspTime + vestLatencyDelay;
                elevatorGoingUpAudio.source.PlayScheduled(exactPlayTime);
            }
            else
            {
                if (elevatorGoingUpAudio.preventOverlay || elevatorGoingUpAudio.loop)
                {
                    elevatorGoingUpAudio.source.Play();
                }
                else
                {
                    elevatorGoingUpAudio.source.PlayOneShot(elevatorGoingUpAudio.clip);
                }
            }

            yield return new WaitForSeconds(elevatorGoingUpAudio.clip.length + (float)vestLatencyDelay);
        }
    }

    [ContextMenu("Play Elevator Door Open Event")]
    public void PlayElevatorDoorOpen()
    {
        if (AudioManagerVest.Instance != null)
        {
            AudioManagerVest.Instance.ClearVestQueue();

            if (!string.IsNullOrEmpty(elevatorDoorOpenVestName))
            {
                AudioManagerVest.Instance.PlayGlobalVestSound(elevatorDoorOpenVestName);
            }
        }

        if (elevatorDoorOpenAudio != null && elevatorDoorOpenAudio.source != null)
        {
            if (vestLatencyDelay > 0)
            {
                double exactPlayTime = AudioSettings.dspTime + vestLatencyDelay;
                elevatorDoorOpenAudio.source.PlayScheduled(exactPlayTime);
            }
            else
            {
                if (elevatorDoorOpenAudio.preventOverlay || elevatorDoorOpenAudio.loop)
                {
                    elevatorDoorOpenAudio.source.Play();
                }
                else
                {
                    elevatorDoorOpenAudio.source.PlayOneShot(elevatorDoorOpenAudio.clip);
                }
            }
        }
    }
}