using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class MetalonAI : MonsterAI
{
    private MetalonMove metalonMove;

    public float skillCoolDown = 0.0f;

    [SerializeField]
    protected float skillCoolTime = 5.0f;

    protected override void Awake()
    {
        base.Awake();

        metalonMove = GetComponent<MetalonMove>();
    }

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
        while(!isDead)
        {
            yield return checkStateTime;

            if (isDamaged) yield return checkStateTime;

            animator.SetFloat(hashSpeed, metalonMove.speed);

            switch (state)
            {
                case State.PATROL:
                    metalonMove.patrolling = true;
                    break;
                case State.TRACE:
                    metalonMove.traceTarget = target.position;
                    break;
                case State.ATTACK:
                    metalonMove.Stop();

                    if (skillCoolDown > 5.0f)
                    {
                        Attack("MetalonSkill", AttackType.SKILL);
                        skillCoolDown = 0.0f;
                    }
                    else if (coolDown > 2.0f)
                    {
                        Attack("MetalonAttack", AttackType.NORMAL);
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
            skillCoolDown += Time.deltaTime;
        }
    }

    private void MetalonStabSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.METALON_STAB, transform.position);
    }

    private void MetalonStabVoice()
    {
        SoundManager.instance.PlaySFX(SoundKey.METALON_STAB_VOICE, transform.position);
    }

    private void MetalonSmashSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.METALON_SMASH, transform.position);
    }

    private void MetalonSmashVoice()
    {
        SoundManager.instance.PlaySFX(SoundKey.METALON_SMASH_VOICE, transform.position);
    }

    private void MetalonDead()
    {
        SoundManager.instance.PlaySFX(SoundKey.METALON_DEAD, transform.position);
    }

    private void MetalonWalkSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.METALON_WALK, transform.position);
    }
}
