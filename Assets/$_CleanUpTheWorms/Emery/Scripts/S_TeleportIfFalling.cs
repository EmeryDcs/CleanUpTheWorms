using UnityEngine;

public class S_TeleportIfFalling : MonoBehaviour
{
	Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
	}

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10f)
		{
			transform.position = startPosition;
		}
	}
}
