using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAI : MonoBehaviour
{
    private enum State
    {
        IDLE, TRACE, ATTACK, DAMAGED, DEAD
    }

    [SerializeField]
    private float attackRange = 2.0f;
    [SerializeField]
    private float traceRange = 10.0f;

    private bool isAttack = false;
    private bool isDamaged = false;
    private bool isDead = false;

    private State state = State.IDLE;

    private Transform target;

    private Animator animator;
    private DragonMove dragonMove;

    private WaitForSeconds checkStateTime;

    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashAttack = Animator.StringToHash("DragonAttack");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        dragonMove = GetComponent<DragonMove>();

        checkStateTime = new WaitForSeconds(0.1f);
    }

    private void Start()
    {
        //target = GameObject.FindGameObjectWithTag("Player").transform;

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

            animator.SetFloat(hashSpeed, dragonMove.speed);

            if (target == null)
            {
                yield return null;
                continue;
            }

            switch (state)
            {
                case State.TRACE:
                    dragonMove.traceTarget = target.position;
                    break;
                case State.ATTACK:
                    dragonMove.Stop();
                    Attack();
                    break;
                case State.DAMAGED:
                    break;
                case State.DEAD:
                    break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        target = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.gameObject == target.gameObject)
        {
            target = null;
            state = State.IDLE;
            //animator.CrossFade("idle01", 0.2f);
            dragonMove.Stop();
        }

    }

    private void Attack()
    {
        if (isAttack) return;

        animator.SetTrigger(hashAttack);
        isAttack = true;
    }

    private void EndAttack()
    {
        isAttack = false;
        state = State.IDLE;
    }
}
