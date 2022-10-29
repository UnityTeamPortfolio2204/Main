using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonAI : MonsterAI
{
    private readonly int hashScreaming = Animator.StringToHash("DragonScream");

    [SerializeField]
    protected float clawCoolTime = 5.0f;
    public float clawCoolDown = 0.0f;
    [SerializeField]
    protected float flameCoolTime = 8.0f;
    public float flameCoolDown = 0.0f;
    [SerializeField]
    protected float flyFlameCoolTime = 10.0f;
    public float flyFlameCoolDown = 0.0f;

    private bool isScreaming = false;

    private float rotSpeed = 1.0f;

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

            if (state == State.ATTACK)
            {
                yield return null;
                continue;
            }

            if(state == State.SCREAM)
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
                    monsterMove.Stop();
                    break;
                case State.SCREAM:
                    Screaming();
                    monsterMove.Stop();
                    break;
                case State.TRACE:
                    monsterMove.traceTarget = target.position;
                    break;
                case State.ATTACK:
                    monsterMove.Stop();

                    if (isAttack)
                    {
                        yield return null;
                        continue;
                    }

                    if(flyFlameCoolDown > flyFlameCoolTime)
                    {
                        Attack("DragonFlyFlame", AttackType.SKILL3);
                        flyFlameCoolDown = 0.0f;
                        SoundManager.instance.PlaySFX(SoundKey.DRAGON_ROAR, transform.position);
                    }
                    else if(flameCoolDown > flameCoolTime)
                    {
                        Attack("DragonFlameAttack", AttackType.SKILL2);
                        flameCoolDown = 0.0f;
                    }
                    else if(clawCoolDown > clawCoolTime)
                    {
                        Attack("DragonClawAttack", AttackType.SKILL);
                        clawCoolDown = 0.0f;
                    }
                    else if(coolDown > coolTime)
                    {
                        Attack("DragonNormalAttack", AttackType.NORMAL);
                        SoundManager.instance.PlaySFX(SoundKey.DRAGON_BITE, transform.position);
                        coolDown = 0.0f;
                    }
                    break;
                case State.DEAD:
                    flameParticle.Stop();
                    this.photonView.RPC("Dead", Photon.Pun.RpcTarget.All);
                    break;
            }
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (!isAttack)
        {
            coolDown += Time.deltaTime;
            clawCoolDown += Time.deltaTime;
            flameCoolDown += Time.deltaTime;
            flyFlameCoolDown += Time.deltaTime;

            if (target != null)
            {
                Vector3 direction = target.transform.position - transform.position;
                Quaternion rot = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttack) return;

        if (target != null) return;

        if(other.CompareTag("Player"))
        {
            this.photonView.RPC("SetState", Photon.Pun.RpcTarget.All, State.SCREAM);
        }
    }

    private void Screaming()
    {
        if (isScreaming) return;

        isScreaming = true;
        animator.SetTrigger(hashScreaming);

        SoundManager.instance.PlaySFX(SoundKey.DRAGON_ROAR, transform.position);
    }

    private void EndScreaming()
    {
        isScreaming = false;
        this.photonView.RPC("SetState", Photon.Pun.RpcTarget.All, State.IDLE);
    }

    private void FlameOn()
    {
        if (flameParticle.CompareTag("Flame"))
        {
            flameParticle.Play();
            EnableAttackCollider();
        }

        SoundManager.instance.PlaySFX(SoundKey.DRAGON_FLAME, transform.position);
    }

    private void FlameOff()
    {
        if (flameParticle.CompareTag("Flame"))
        {
            flameParticle.Stop();
            DisableAttackCollider();
        }
    }

    private void WingSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.DRAGON_WING, transform.position);
    }

    private void StepSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.DRAGON_STEP, transform.position);
    }

    private void ClawSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.DRAGON_CLAW, transform.position);
    }

    private void DeadSound()
    {
        SoundManager.instance.PlaySFX(SoundKey.DRAGON_DEAD, transform.position);
    }
}
