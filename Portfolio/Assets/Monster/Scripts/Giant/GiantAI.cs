using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GiantAI : MonsterAI, IPunObservable
{ 
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashAttack = Animator.StringToHash("GiantAttack");


    private GiantAttackCollider attackCollider;

    protected override void Awake()
    {
        base.Awake();

        attackCollider = GetComponentInChildren<GiantAttackCollider>();
    }

    private void Start()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        attackCollider.gameObject.GetComponent<SphereCollider>().enabled = false;
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

            animator.SetFloat(hashSpeed, monsterMove.speed);

            if (target == null)
            {
                yield return null;
                continue;
            }

            switch (state)
            {
                case State.TRACE:
                    monsterMove.traceTarget = target.position;
                    break;
                case State.ATTACK:
                    monsterMove.Stop();
        
                    if (coolDown > 3.0f)
                    { 
                        Attack();
                        coolDown = 0.0f;
                    }
                    break;
                case State.DAMAGED:
                    break;
                case State.DEAD:
                    break;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (isDead) return;

        if (!other.CompareTag("Player")) return;

        if (other.gameObject == target.gameObject)
        {
            target = null;
            state = State.IDLE;
            animator.CrossFade("Giant@Idle01", 0.2f);
            monsterMove.Stop();
        }
            
    }

    private void Update()
    {
        if (isDead) return;

        coolDown += Time.deltaTime;
    }

    protected override void Attack()
    {
        base.Attack();

        animator.SetTrigger(hashAttack);
    }

    private void EnableCollider()
    {
        attackCollider.gameObject.GetComponent<SphereCollider>().enabled = true;
    }

    protected override void EndAttack()
    {
        base.EndAttack();
        state = State.IDLE;

        attackCollider.gameObject.GetComponent<SphereCollider>().enabled = false;
    }

    
/*    public override void Damaged(float damage)
    {
        if (isDead) return;

        if (curHp < 0.0f)
        {
*//*            PhotonView pv = this.photonView;
            pv.RPC("Dead", RpcTarget.All);*//*
            Dead();
        }
            

        base.Damaged(damage);

        animator.SetTrigger(hashDamaged);
    }
*/
    
    /*protected override void Dead()
    {
        base.Dead();

        animator.SetTrigger(hashDead);
    }*/

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {

        }
    }
}
