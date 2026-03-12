using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_VoiceOver : MonoBehaviour
{
    public static S_VoiceOver Instance;

    [SerializeField] private Sound[] speechList;

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

        if (speechList != null)
        {
            for (int i = 0; i < speechList.Length; i++)
            {
                InitializeSound(speechList[i], "SequenceSpeechAudioSource_" + i);
            }
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
        s.source.loop = false;
        s.source.playOnAwake = false;
    }

    public void PlayAudio(int index)
    {
        if (speechList == null || index < 0 || index >= speechList.Length) return;

        for (int i = 0; i < speechList.Length; i++)
        {
            if (speechList[i] != null && speechList[i].source != null)
            {
                speechList[i].source.Stop();
            }
        }

        Sound currentSpeech = speechList[index];

        if (currentSpeech != null && currentSpeech.source != null)
        {
            currentSpeech.source.Play();
        }
    }
}