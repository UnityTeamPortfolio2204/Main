using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantAI : MonsterAI
{ 
    private void Start()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }

    private IEnumerator CheckState()
    {
        while(!isDead)
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
                    this.photonView.RPC("SetState", Photon.Pun.RpcTarget.All, State.ATTACK);
                }
                else
                {
                    this.photonView.RPC("SetState", Photon.Pun.RpcTarget.All, State.TRACE);
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

            animator.SetFloat(hashSpeed, monsterMove.speed);

            if (target == null)
            {
                yield return null;
                continue;
            }

            switch (state)
            {
                case State.IDLE:
                    animator.CrossFade("Giant@Idle01", 0.2f);
                    monsterMove.Stop();
                    break;
                case State.TRACE:
                    monsterMove.traceTarget = target.position;
                    break;
                case State.ATTACK:
                    monsterMove.Stop();
        
                    if (coolDown > 3.0f)
                    { 
                        Attack("GiantAttack", AttackType.NORMAL);
                        coolDown = 0.0f;
                    }
                    break;
                case State.DEAD:
                    this.photonView.RPC("Dead", Photon.Pun.RpcTarget.All);
                    break;
            }
        }
    }

    private void Update()
    {
        if (isDead) return;

        if(!isAttack && target != null)
            coolDown += Time.deltaTime;
    }

    private void GiantStepSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.GIANT_STEP, transform.position);
    }

    private void GiantAttackSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.GIANT_ATTACK, transform.position);
    }
}
