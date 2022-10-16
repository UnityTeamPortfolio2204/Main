using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MetalonMove : MonsterMove
{
    private bool _patrolling;
    public bool patrolling
    {
        get { return _patrolling; }
        set
        {
            if(value)
            {
                agent.speed = patrolSpeed;
                agent.isStopped = false;
                if(!_patrolling)
                {
                    SetPatrolPos();
                }
            }
            _patrolling = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        agent.speed = patrolSpeed;
    }

    protected override void Update()
    {
        base.Update();
        if (!_patrolling) return;

        if(agent.remainingDistance <= 1.0f)
        {
            SetPatrolPos();
        }
    }

    private void SetPatrolPos()
    {
        Vector3 pos = transform.position;
        pos.x = Random.Range(pos.x - patrolRange, pos.x + patrolRange);
        pos.z = Random.Range(pos.z - patrolRange, pos.z + patrolRange);

        agent.destination = pos;
    }
}
