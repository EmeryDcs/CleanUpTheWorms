using UnityEngine;
using System.Collections;

public class ElevatorAudio : MonoBehaviour
{
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
        float waitTime = 0f;

        if (elevatorGoingUpAudio != null && elevatorGoingUpAudio.source != null)
        {
            waitTime = elevatorGoingUpAudio.clip.length;

            if (elevatorGoingUpAudio.preventOverlay || elevatorGoingUpAudio.loop)
            {
                elevatorGoingUpAudio.source.Play();
            }
            else
            {
                elevatorGoingUpAudio.source.PlayOneShot(elevatorGoingUpAudio.clip);
            }
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(elevatorGoingUpVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(elevatorGoingUpVestName);
        }

        yield return new WaitForSeconds(waitTime);

        PlayElevatorDoorOpen();
    }

    [ContextMenu("Play Elevator Door Open Event")]
    public void PlayElevatorDoorOpen()
    {
        if (elevatorDoorOpenAudio != null && elevatorDoorOpenAudio.source != null)
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

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(elevatorDoorOpenVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(elevatorDoorOpenVestName);
        }
    }
}