using UnityEngine;
using UnityEngine.AI;
using MyTools.UnityEvents;
using Unity.Mathematics;

namespace MyTools.Navigation
{
    /// <summary>
    /// Controls falling and landing behavior for a GameObject with a NavMeshAgent, enabling the agent to simulate
    /// falling and trigger related events.
    /// </summary>
    /// <remarks>Require NavMeshAgent component to work. Rigidbody is also required for trigger events in AgentFallRegion.
    /// Rigidbody should be set to isKinematic.</remarks>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavAgentFallController : MonoBehaviour
    {
        private const float MAX_FALL_SPEED = 50f;
        private const float FALL_ACCELERATION = 9.81f;

        public bool isFalling { get; private set; } = false;
        public float yBeforeFall { get; private set; } = 0f;
        public float fallDistance => isFalling ? (yBeforeFall - transform.position.y) : 0f;

        public VoidEvent startFallEvent = new VoidEvent();
        public FloatEvent landEvent = new FloatEvent();

        private float colliderYExtent = 0f;
        private Vector3 inAirMovement = Vector3.zero;

        private NavMeshAgent agentRef;

        private void Awake()
        {
            agentRef = GetComponent<NavMeshAgent>();
            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                colliderYExtent = collider.bounds.extents.y;
            }
            else
            {
                Debug.LogWarning("NavAgentFallController: No collider found on the GameObject. " +
                    "Falling detection may not work properly.", this);
            }
        }

        private void Update()
        {
            if (isFalling)
            {
                inAirMovement.y = Mathf.Max(-MAX_FALL_SPEED, inAirMovement.y - FALL_ACCELERATION * Time.deltaTime);
                Vector3 curInAirMovement = new Vector3(
                    inAirMovement.x * Time.deltaTime,
                    inAirMovement.y * Time.deltaTime,
                    inAirMovement.z * Time.deltaTime);
                float curFallDistance = -curInAirMovement.y;

                // land detection
                if (Physics.Raycast(
                    transform.position,
                    Vector3.down,
                    out RaycastHit hitInfo,
                    colliderYExtent + Mathf.Max(curFallDistance, 0) - 0.1f))  // -0.1f to prevent instant land
                {
                    if (NavMesh.SamplePosition(hitInfo.point, out NavMeshHit navHitInfo, 0.2f, NavMesh.AllAreas))
                    {
                        transform.position = navHitInfo.position + Vector3.up * colliderYExtent;
                        Land();
                        return;
                    }
                }

                transform.position += curInAirMovement;
            }
        }

        public void StartFalling()
        {
            StartFalling(Vector3.zero);
        }

        public void StartFalling(Vector3 initialVelocity)
        {
            if (isFalling) return;

            startFallEvent.Invoke();
            SetIsFalling(true);
            yBeforeFall = transform.position.y;
            inAirMovement = initialVelocity;
            Debug.Log("Start falling with initial velocity: " + initialVelocity);
        }

        private void Land()
        {
            if (!isFalling) return;

            landEvent.Invoke(yBeforeFall - transform.position.y);
            Debug.Log("Landed with: " + (yBeforeFall - transform.position.y));
            SetIsFalling(false);
        }

        private void SetIsFalling(bool value)
        {
            isFalling = value;
            agentRef.enabled = !value;
        }
    }
}