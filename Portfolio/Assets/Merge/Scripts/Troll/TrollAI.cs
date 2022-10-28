using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollAI : MonsterAI
{
    private TrollMove trollMove;

    public float smashCoolDown = 0.0f;

    [SerializeField]
    protected float smashCoolTime = 5.0f;

    protected override void Awake()
    {
        base.Awake();

        trollMove = GetComponent<TrollMove>();
    }

    private void Start()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }

    private IEnumerator CheckState()
    {
        while (!isDead)
        {
            if (state == State.DEAD) yield break;
           
            if (isDamaged) yield return checkStateTime;

            if (state == State.ATTACK)
            {
                yield return null;
                continue;
            }

            if (target != null)
            {
                float distance = Vector3.Distance(target.position, transform.position);

                if (distance < attackRange)
                {
                    state = State.ATTACK;
                }
                else
                {
                    state = State.TRACE;
                }
            }

            yield return checkStateTime;
          
        }
    }

    private IEnumerator Action()
    {
        while (!isDead)
        {
            yield return checkStateTime;

            if (isDamaged) yield return checkStateTime;

            animator.SetFloat(hashSpeed, trollMove.speed);

            switch (state)
            {
                case State.PATROL:
                    trollMove.patrolling = true;
                    break;
                case State.TRACE:
                    trollMove.traceTarget = target.position;
                    break;
                case State.ATTACK:
                    trollMove.Stop();

                    if (smashCoolDown > 5.0f)
                    {
                        Attack("TrollSmash", AttackType.SKILL);
                        smashCoolDown = 0.0f;
                    }
                    else if (coolDown > 2.0f)
                    {
                        Attack("TrollAttack", AttackType.NORMAL);
                        coolDown = 0.0f;
                    }

                    break;
                case State.DEAD:
                    Dead();
                    break;
            }
        }
    }

    private void Update()
    {
        CheckDead();

        if (!isAttack && target != null && !isDead)
        {
            coolDown += Time.deltaTime;
            smashCoolDown += Time.deltaTime;
        }
    }
}
