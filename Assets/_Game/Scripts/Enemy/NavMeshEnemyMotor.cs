using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshEnemyMotor : MonoBehaviour, IEnemyMove
{
    [SerializeField]
    private NavMeshAgent _agent;

    private float _durability;

    public void Move(Vector3 position)
    {
        if (this._agent != null)
        {
            this._agent = this.GetComponent<NavMeshAgent>();
        }

        if (this._durability > 0)
        {
            this._agent.SetDestination(position);
        }

    }

    public void SetDurability(float durability)
    {
        this._durability = durability;
    }
}
