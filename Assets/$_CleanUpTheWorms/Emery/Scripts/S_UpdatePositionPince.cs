using UnityEngine;

public class S_UpdatePositionPince : MonoBehaviour
{
	[SerializeField]
	Transform anchor;

	public void UpdatePositionPince()
	{
		transform.position = anchor.position;
	}
}
