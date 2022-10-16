using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public enum State
    {
        PATROL, IDLE, TRACE, ATTACK, DAMAGED, DEAD
    }

    [SerializeField]
    protected float attackRange = 3.0f;
    [SerializeField]
    protected float traceRange = 7.0f;
    [SerializeField]
    protected float MaxHp = 100.0f;
    [SerializeField]
    protected float curHp = 100.0f;

    protected bool isDead = false;
    protected bool isDamaged = false;
    protected bool isAttack = false;

    protected State state = State.PATROL;

    protected Transform target;

    protected Animator animator;
    protected MonsterMove monsterMove;

    protected WaitForSeconds checkStateTime;

    [SerializeField]
    protected float coolDown = 0.0f;
    [SerializeField]
    protected float skillCoolDown = 0.0f;

    virtual protected void Awake()
    {
        animator = GetComponent<Animator>();
        monsterMove = GetComponent<MonsterMove>();

        checkStateTime = new WaitForSeconds(0.1f);
    }

    virtual protected void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        target = other.transform;
    }

    virtual protected void Attack()
    {
        if (isAttack) return;

        coolDown = 0.0f;
        isAttack = true;
    }

    virtual protected void EndAttack()
    {
        isAttack = false;
    }

    virtual public void Damaged(float damage)
    {
        if (isDead) return;

        if (isAttack)
            isAttack = false;

        isDamaged = true;
        curHp -= damage;

        monsterMove.Stop();
    }

    virtual protected void EndDamaged()
    {
        isDamaged = false;
    }

    virtual protected void Dead()
    {
        isDead = true;
        monsterMove.Stop();

        state = State.DEAD;
    }
}
