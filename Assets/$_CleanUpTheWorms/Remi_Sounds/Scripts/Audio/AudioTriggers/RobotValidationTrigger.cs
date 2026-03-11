using UnityEngine;

public class RobotValidationTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (grab.Instance != null && grab.Instance.GetGrabbedElm() == null)
            return;

        if (other.CompareTag("GrabbableElm"))
        {
            if (RobotAudio.Instance != null)
            {
                RobotAudio.Instance.PlayValidation();

                FindFirstObjectByType<S_RobotAnim>().SetAnimOpening(false);
            }
        }
    }
}