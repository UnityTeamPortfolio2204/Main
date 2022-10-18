using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;


public class MetalonAI : MonsterAI
{
    private MetalonMove metalonMove;

    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashStabAttack = Animator.StringToHash("MetalonAttack");
    private readonly int hashSkillAttack = Animator.StringToHash("MetalonSkill");
    private readonly int hashDamaged = Animator.StringToHash("Damaged");
    private readonly int hashDead = Animator.StringToHash("MetalonDead");

    private MetalonAttackCollider attackCollider;
    private MetalonSkillCollider skillCollider;

    protected override void Awake()
    {
        base.Awake();

        metalonMove = GetComponent<MetalonMove>();

        attackCollider = GetComponentInChildren<MetalonAttackCollider>();
        skillCollider = GetComponentInChildren<MetalonSkillCollider>();
    }

    private void Start()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        attackCollider.gameObject.GetComponent<SphereCollider>().enabled = false;
        skillCollider.gameObject.GetComponent<SphereCollider>().enabled = false;
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
                        SkillAttack();
                    }
                    else if (coolDown > 2.0f)
                    {
                        Attack();
                    }
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
            state = State.PATROL;
        }
    }

    private void Update()
    {
        coolDown += Time.deltaTime;
        skillCoolDown += Time.deltaTime;
    }

    protected override void Attack()
    {
        if (isDead) return;

        if (isAttack) return;

        base.Attack();

        animator.SetTrigger(hashStabAttack);

        attackCollider.gameObject.GetComponent<SphereCollider>().enabled = true;
    }

    private void SkillAttack()
    {
        if (isAttack) return;

        skillCoolDown = 0.0f;
        isAttack = true;
        animator.SetTrigger(hashSkillAttack); 
    }

    private void EnableSkillCollider()
    {
        skillCollider.gameObject.GetComponent<SphereCollider>().enabled = true;
    }

    protected override void EndAttack()
    {
        base.EndAttack();
        state = State.PATROL;

        attackCollider.gameObject.GetComponent<SphereCollider>().enabled = false;
        skillCollider.gameObject.GetComponent<SphereCollider>().enabled = false;
    }

/*    public override void Damaged(float damage)
    {
        if (isDead) return;

        if (curHp < 0.0f)
            Dead();

        base.Damaged(damage);
        animator.SetTrigger(hashDamaged);
    }*/

/*    protected override void Dead()
    {
        base.Dead();

        animator.SetTrigger(hashDead);

*//*        Item TEMP = this.gameObject.GetComponent<Item>();
        TEMP.gameObject.SetActive(true);
        TEMP.gameObject.transform.position = this.gameObject.transform.position + new Vector3(1, 0, 0);
*//*
    }*/
}
