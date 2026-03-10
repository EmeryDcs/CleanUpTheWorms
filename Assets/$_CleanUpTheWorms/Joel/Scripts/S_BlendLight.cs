using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_BlendLight : MonoBehaviour
{
	UnityEngine.Rendering.ProbeReferenceVolume probeRefVolume;
	public string scenario01 = "Scenario01Name";
	public string scenario02 = "Scenario02Name";
	public string scenario03 = "Scenario03Name";
	[Range(0, 1)] public float blendingFactor = 0.5f;
	[Min(1)] public int numberOfCellsBlendedPerFrame = 10;

	void Start()
	{
		probeRefVolume = UnityEngine.Rendering.ProbeReferenceVolume.instance;
		probeRefVolume.lightingScenario = scenario01;
		probeRefVolume.numberOfCellsBlendedPerFrame = numberOfCellsBlendedPerFrame;
	}

	public void SetScenario(int i)
	{
		switch (i)
		{
			case 0:
				probeRefVolume.lightingScenario = scenario01;
				break;
			case 1:
				probeRefVolume.lightingScenario = scenario02;
				break;
			case 2:
				probeRefVolume.lightingScenario = scenario03;
				break;
			default:
				Debug.LogError("Invalid scenario index: " + i);
				break;
		}
	}
}