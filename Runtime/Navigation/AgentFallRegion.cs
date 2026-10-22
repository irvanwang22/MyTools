using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace MyTools.Navigation
{
    /// <summary>
    /// Defines a region that triggers agent falling behavior when agents with a NavAgentFallController enter or exit
    /// the area. Manages enabling and disabling fall logic for agents within the region.
    /// </summary>
    /// <remarks>Attach this component to a GameObject with a NavMeshObstacle and a trigger collider to
    /// designate an area where agent fall behavior should be controlled. The region interacts with agents that have a
    /// NavAgentFallController component, enabling or disabling their fall state as appropriate. Use EnableFall,
    /// DisableFall, or ToggleFall to control the fall state for all agents currently within the region.</remarks>
    [RequireComponent(typeof(NavMeshObstacle))]
    public class AgentFallRegion : MonoBehaviour
    {
        private bool isFallEnabled => obstacleRef.enabled;
        private HashSet<NavAgentFallController> fallControllers = new();

        private NavMeshObstacle obstacleRef;

        private void Awake()
        {
            obstacleRef = GetComponent<NavMeshObstacle>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var controller = other.GetComponentInParent<NavAgentFallController>();
            if (controller != null)
            {
                fallControllers.Add(controller);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var controller = other.GetComponentInParent<NavAgentFallController>();
            if (controller != null)
            {
                fallControllers.Remove(controller);
            }
        }

        public void EnableFall()
        {
            foreach (var c in fallControllers)
            {
                c.StartFalling();
            }
            obstacleRef.enabled = true;     // enable obstacle after navAgents are disabled
        }

        public void DisableFall()
        {
            obstacleRef.enabled = false;
        }

        public void ToggleFall()
        {
            if (isFallEnabled)
            {
                DisableFall();
            }
            else
            {
                EnableFall();
            }

        }
    }
}