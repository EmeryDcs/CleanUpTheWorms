using UnityEngine;

public class S_LookAtPlayer : MonoBehaviour
{
	GameObject player = null;

	private void Start()
	{
		if (!GameObject.FindGameObjectWithTag("Player"))
			return;
		player = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void Update()
	{
		if (player != null)
		{
			Vector3 direction = transform.position - player.transform.position;
			transform.rotation = Quaternion.LookRotation(direction);
		}
	}
}
