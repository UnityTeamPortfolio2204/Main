
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MonsterAI : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum State
    {
        SCREAM, PATROL, IDLE, WALK, TRACE, ATTACK, DAMAGED, DEAD
    }

    public enum AttackType
    {
        NORMAL, SKILL, SKILL2, SKILL3
    }

    [SerializeField]
    protected float attackRange = 3.0f;
    [SerializeField]
    protected float traceRange = 7.0f;
    [SerializeField]
    protected float MaxHp = 100.0f;
    [SerializeField]
    protected float curHp = 100.0f;

    [SerializeField]
    protected Collider[] attackColliders;

    protected AttackType attackType = AttackType.NORMAL;

    protected bool isDead = false;
    protected bool isDamaged = false;
    protected bool isAttack = false;

    [SerializeField]
    protected State state = State.PATROL;

    protected Transform target;

    protected Animator animator;
    protected MonsterMove monsterMove;

    protected WaitForSeconds checkStateTime;

    public float coolDown = 0.0f;

    [SerializeField]
    protected float coolTime = 3.0f;

    private int hashAttack;
    protected readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDamaged = Animator.StringToHash("Damaged");
    private readonly int hashDead = Animator.StringToHash("Dead");


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

    private void OnTriggerExit(Collider other)
    {
        if (isDead) return;

        if (!other.CompareTag("Player")) return;

        if (other.gameObject == target.gameObject)
        {
            target = null;
            state = State.IDLE;
        }
    }

    protected void Attack(string name, AttackType type)
    {
        if (isDead) return;
        if (isAttack) return;

        isAttack = true;

        hashAttack = Animator.StringToHash(name);

        attackType = type;

        animator.SetTrigger(hashAttack);
    }

    virtual protected void EndAttack()
    {
        isAttack = false;

        state = State.IDLE;

        DisableAttackCollider();
    }

    protected void EnableAttackCollider()
    {
        attackColliders[(int)attackType].enabled = true;
    }

    protected void DisableAttackCollider()
    {
        attackColliders[(int)attackType].enabled = false;
    }

    virtual public void Damaged(float damage)
    {
        if (isDead) return;

        DisableAttackCollider();
       
        if (isAttack)
            isAttack = false;

        PhotonView pv = this.gameObject.GetPhotonView();
        pv.RPC("PDamage", RpcTarget.All, damage);

/*        if (curHp <= 0)
        {
            pv.RPC("Dead", RpcTarget.All);
        }*/

/*        isDamaged = true;
        curHp -= damage;
       
        monsterMove.Stop();
       
        animator.SetTrigger(hashDamaged);*/
    }

    virtual protected void EndDamaged()
    {
        isDamaged = false;
    }

    virtual protected void Dead()
    {
        isDead = true;
        monsterMove.Stop();

        animator.SetTrigger(hashDead);

        DisableAttackCollider();

        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(8.0f);

        Destroy(gameObject);
    }

    protected void CheckDead()
    {
        if (curHp <= 0)
            state = State.DEAD;
    }

/*    [PunRPC]
    public void Dead()
    {
        isDead = true;
        monsterMove.Stop();

        state = State.DEAD;
        animator.SetTrigger(hashDead);
    }*/

    [PunRPC]
    public void PDamage(float damage)
    {
        isDamaged = true;
        curHp -= damage;

        monsterMove.Stop();
        animator.SetTrigger(hashDamaged);
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
}
