using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _transform;
    [SerializeField] private float _moveSpeed;

    public void Update()
    {
        if (_agent.isOnNavMesh == false)
            return;

        _agent.Move(_transform.forward * _moveSpeed * Time.deltaTime);
    }

    public void ResetAgent()
    {
        _agent.enabled = false;
        _agent.Warp(_transform.position);
        _agent.enabled = true;
    }
}