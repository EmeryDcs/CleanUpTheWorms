using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_BlendLight : MonoBehaviour
{
    public static S_BlendLight instance { get; private set; }

    UnityEngine.Rendering.ProbeReferenceVolume probeRefVolume;
    public string scenario01 = "Scenario01Name";
    public string scenario02 = "Scenario02Name";
    public string scenario03 = "Scenario03Name";
    [Range(0, 1)] public float blendingFactor = 0.5f;
    [Min(1)] public int numberOfCellsBlendedPerFrame = 10;

    private Coroutine blendCoroutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        probeRefVolume = UnityEngine.Rendering.ProbeReferenceVolume.instance;
        probeRefVolume.lightingScenario = scenario01;
        probeRefVolume.numberOfCellsBlendedPerFrame = numberOfCellsBlendedPerFrame;
    }

    public void SetScenario(string s, float transitionTime = 0f)
    {
        if (transitionTime <= 0f)
        {
            if (blendCoroutine != null) StopCoroutine(blendCoroutine);
            probeRefVolume.lightingScenario = s;
        }
        else
        {
            if (blendCoroutine != null) StopCoroutine(blendCoroutine);
            blendCoroutine = StartCoroutine(BlendScenarioRoutine(s, transitionTime));
        }
    }

    private IEnumerator BlendScenarioRoutine(string targetScenario, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float factor = Mathf.Clamp01(elapsed / duration);
            probeRefVolume.BlendLightingScenario(targetScenario, factor);
            yield return null;
        }

        probeRefVolume.lightingScenario = targetScenario;
    }
}