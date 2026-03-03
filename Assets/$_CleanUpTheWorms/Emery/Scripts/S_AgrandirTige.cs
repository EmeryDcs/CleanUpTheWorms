using TMPro;
using UnityEngine;

public class S_AgrandirTige : MonoBehaviour
{
	[Header("Taille de la tige min et max")]
	public float minTaille = 0.5f;
	[Tooltip("maxTaille peut évoluer dans le temps pour agrandir la tige au fur et à mesure que le joueur progresse dans le jeu")]
	public float maxTaille = 5f;
	[Header("GameObject tige")]
	public GameObject tige;

	[Header("Distance entre les deux manettes min et max")]
	[SerializeField]
	float distanceMaxGun = 0.3f;
	[SerializeField]
	float distanceMinGun = 0f;

	[Header("Controllers")]
	public GameObject controllerLeft;
	public GameObject controllerRight;

	[Header("Debug")]
	public TextMeshProUGUI textTailleTige;

	// Update is called once per frame
	void Update()
	{
		ResizedStick();
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

	private void ResizedStick()
	{
		float opening = NormalizedDistanceBetweenControllers();
		float newTaille = Mathf.Lerp(minTaille, maxTaille, opening);

		textTailleTige.text = $"Distance: {DistanceBetweenControllers():F2} m\nNormalized: {opening:F2}\nSize : {newTaille:F2}";

		tige.transform.localScale = new Vector3(tige.transform.localScale.x, newTaille, tige.transform.localScale.z);
	}
}
