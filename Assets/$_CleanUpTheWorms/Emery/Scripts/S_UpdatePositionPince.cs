using UnityEngine;
using UnityEngine.Rendering;

public class S_UpdatePositionPince : MonoBehaviour
{
	[SerializeField]
	Transform anchor;

    [SerializeField] GameObject[] claws;

    private void Start()
    {
        foreach(GameObject claw in claws)
        {
            claw.transform.SetParent(transform);
        }
    }

    public void UpdatePositionPince()
	{
		transform.position = anchor.position;
	}
}
