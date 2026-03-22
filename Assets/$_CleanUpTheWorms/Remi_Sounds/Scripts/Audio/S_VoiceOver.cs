using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_VoiceOver : MonoBehaviour
{
    public static S_VoiceOver Instance;

    public bool isEnglish;

    [SerializeField] private Sound[] speechList;
    [SerializeField] private Sound[] speechListEnglish;

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

        if (System.Environment.GetCommandLineArgs().Contains("-isinenglish"))
        {
            isEnglish = true;
        }

        if (speechList != null)
        {
            for (int i = 0; i < speechList.Length; i++)
            {
                InitializeSound(speechList[i], "SequenceSpeechAudioSource_" + i);
            }
        }

        if (speechListEnglish != null)
        {
            for (int i = 0; i < speechListEnglish.Length; i++)
            {
                InitializeSound(speechListEnglish[i], "SequenceSpeechAudioSource_EN_" + i);
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
        if (speechList != null)
        {
            for (int i = 0; i < speechList.Length; i++)
            {
                if (speechList[i] != null && speechList[i].source != null)
                {
                    speechList[i].source.Stop();
                }
            }
        }

        if (speechListEnglish != null)
        {
            for (int i = 0; i < speechListEnglish.Length; i++)
            {
                if (speechListEnglish[i] != null && speechListEnglish[i].source != null)
                {
                    speechListEnglish[i].source.Stop();
                }
            }
        }

        Sound[] currentList = isEnglish ? speechListEnglish : speechList;

        if (currentList == null || index < 0 || index >= currentList.Length) return;

        Sound currentSpeech = currentList[index];

        if (currentSpeech != null && currentSpeech.source != null)
        {
            currentSpeech.source.Play();
        }
    }
}