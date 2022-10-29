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

            animator.SetFloat(hashSpeed, trollMove.speed);

            switch (state)
            {
                case State.IDLE:
                    state = State.PATROL;
                    break;
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
                    this.photonView.RPC("Dead", Photon.Pun.RpcTarget.All);
                    break;
            }
        }
    }

    private void Update()
    {

        if (!isAttack && target != null && !isDead)
        {
            coolDown += Time.deltaTime;
            smashCoolDown += Time.deltaTime;
        }
    }

    private void TrollStepSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.TROLL_STEP, transform.position);
    }

    private void TrollAttackSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.TROLL_ATTACK, transform.position);
    }

    private void TrollSmashSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.TROLL_SMASH, transform.position);
    }

    private void TrollSmashVoice()
    {
        SoundManager.instance.PlaySFX(SoundKey.TROLL_SMASH_VOICE, transform.position);
    }

    private void TrollDamagedSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.TROLL_DAMAGED, transform.position);
    }

    private void TrollDeadSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.TROLL_DEAD, transform.position);
    }
}
