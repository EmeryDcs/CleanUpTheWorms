using TMPro;
using UnityEngine;

public class S_DistanceBetweenControllers : MonoBehaviour
{
	[Header("Distance entre les deux pinces min et max")]
	public float distanceMin = 0.5f;
	public float distanceMax = 1.5f;
	[Header("GameObjects Pinces")]
	public GameObject pliersLeft;
	public GameObject pliersRight;

	[Header("Distance entre les deux manettes min et max")]
	[SerializeField]
	float distanceMaxGun = 0.3f;
	[SerializeField]
	float distanceMinGun = 0f;

	[Header("Controllers")]
	public GameObject controllerLeft;
	public GameObject controllerRight;

	[Header("Debug")]
	public TextMeshProUGUI distanceText;

	// Update is called once per frame
	void Update()
	{
		CalculateOpeningPliers();
	}

	private float DistanceBetweenControllers()
	{
		return Vector3.Distance(controllerRight.transform.position, controllerLeft.transform.position);
	}

	private float NormalizedDistanceBetweenControllers()
	{
		float distance = DistanceBetweenControllers();
		return Mathf.InverseLerp(distanceMinGun, distanceMaxGun, distance);
	}

	private void CalculateOpeningPliers()
	{
		float opening = NormalizedDistanceBetweenControllers();
		pliersLeft.transform.localPosition = new Vector3(-opening, 0, 0);
		pliersRight.transform.localPosition = new Vector3(opening, 0, 0);

		distanceText.text = $"Distance: {DistanceBetweenControllers():F2} m\nNormalized: {opening:F2}";
	}
}
