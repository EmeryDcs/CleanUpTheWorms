using UnityEngine;
using UnityEngine.AI;

public class AILarvaAnimation : MonoBehaviour
{
    public bool isDying = false;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private AILarva aiLarva;
    [SerializeField] private float movementThreshold = 0.0001f;

    private Animator animator;
    private Vector3 previousPosition;

    private readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private readonly int HasBeenCaughtHash = Animator.StringToHash("hasBeenCaught");
    private readonly int IsDyingHash = Animator.StringToHash("isDying");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    private void Update()
    {
        if (animator == null) return;

        animator.SetBool(IsDyingHash, isDying);

        if (!isDying)
        {
            bool isCaught = UpdateCaughtState();
            UpdateWalkingState(isCaught);
        }

        previousPosition = transform.position;
    }

    private void UpdateWalkingState(bool isCaught)
    {
        bool isMoving = false;

        if (!isCaught && agent != null && agent.enabled && agent.isOnNavMesh)
        {
            float sqrMovement = (transform.position - previousPosition).sqrMagnitude;


            if (sqrMovement > movementThreshold)
            {
                isMoving = true;
            }
        }

        animator.SetBool(IsWalkingHash, isMoving);
    }

    private bool UpdateCaughtState()
    {
        bool caught = (agent != null && !agent.enabled);
        animator.SetBool(HasBeenCaughtHash, caught);
        return caught;
    }
}