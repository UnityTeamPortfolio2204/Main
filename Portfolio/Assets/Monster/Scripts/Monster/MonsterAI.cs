using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MonsterAI : MonoBehaviourPunCallbacks, IPunObservable
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
    private readonly int hashDamaged = Animator.StringToHash("Damaged");
    private readonly int hashDead = Animator.StringToHash("Dead");

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

        PhotonView pv = this.gameObject.GetPhotonView();
        pv.RPC("PDamage", RpcTarget.All, damage);
        /*        isDamaged = true;
                curHp -= damage;

                monsterMove.Stop();*/
        if (curHp <= 0)
        {
            pv.RPC("Dead", RpcTarget.All);
        }
        animator.SetTrigger(hashDamaged);
    }

    virtual protected void EndDamaged()
    {
        isDamaged = false;
    }

    [PunRPC]
    public void Dead()
    {
        isDead = true;
        monsterMove.Stop();

        state = State.DEAD;
        animator.SetTrigger(hashDead);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
        }
        else
        {
        }
    }

    [PunRPC]
    public void PDamage(float damage)
    {
        isDamaged = true;
        curHp -= damage;

        monsterMove.Stop();
    }
}
