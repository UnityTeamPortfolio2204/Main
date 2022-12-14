
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class MonsterAI : MonoBehaviourPun, IPunObservable
{
    public enum State
    {
        SCREAM, PATROL, IDLE, WALK, TRACE, ATTACK, DAMAGED, DEAD
    }

    public enum AttackType
    {
        NORMAL, SKILL, SKILL2, SKILL3
    }

    #region Serialize Field
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
    [SerializeField]
    protected State state = State.PATROL;
    [SerializeField]
    protected float coolTime = 3.0f;
    #endregion

    #region Protected Field
    protected AttackType attackType = AttackType.NORMAL;
    protected bool isAttack = false;
    protected bool isDead = false;
    protected Transform target;
    protected Animator animator;
    protected MonsterMove monsterMove;
    protected WaitForSeconds checkStateTime;
    protected readonly int hashSpeed = Animator.StringToHash("Speed");
    protected ParticleSystem flameParticle;
    protected bool isTarget = false;
    #endregion

    #region Private Field
    private int hashAttack;
    private readonly int hashDamaged = Animator.StringToHash("Damaged");
    private readonly int hashDead = Animator.StringToHash("Dead");
    #endregion

    #region Public Field
    public bool isDamaged = false;
    public float coolDown = 0.0f;
    #endregion


    #region Monobehaviour Callbacks
    virtual protected void Awake()
    {
        animator = GetComponent<Animator>();
        monsterMove = GetComponent<MonsterMove>();
        flameParticle = GetComponentInChildren<ParticleSystem>();

        checkStateTime = new WaitForSeconds(0.1f);
    }

    virtual protected void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isTarget) return;

        target = other.transform;
        this.photonView.RPC("SetTarget", RpcTarget.All, true);
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (isDead) return;

        if (!other.CompareTag("Player")) return;
        

        if (other.transform == target.transform)
        {
            target = null;
            this.photonView.RPC("SetState", RpcTarget.All, State.IDLE);
            this.photonView.RPC("SetTarget", RpcTarget.All, false);
        }
    }
    #endregion

    #region Protected Methods
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

        this.photonView.RPC("SetState", RpcTarget.All, State.IDLE);

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

    protected void CheckDead()
    {
        if (curHp <= 0)
        {
            this.photonView.RPC("SetState", RpcTarget.All, State.DEAD);
            this.photonView.RPC("Dead", RpcTarget.All);
        }

    }

    virtual protected void EndDamaged()
    {
        
        isDamaged = false;
        isAttack = false;
    }
    #endregion


    #region Public Methods
    virtual public void Damaged(float damage)
    {
        if (isDead) return;
            

        this.photonView.RPC("PDamaged", RpcTarget.All, damage);
    }

    public State GetState()
    {
        return state;
    }

    #endregion

    #region Photon Methods
    [PunRPC]
    protected void PDamaged(float damage)
    {
        DisableAttackCollider();
        isDamaged = true;
        curHp -= damage;

        monsterMove.Stop();

        animator.SetTrigger(hashDamaged);
        SoundManager.instance.PlaySFX(SoundKey.HIT, transform.position);

        CheckDead();
    }



    [PunRPC]
    virtual protected void Dead()
    {
        //PhotonView pv = this.gameObject.GetComponent<ItemThrower>().photonView;
        this.photonView.RPC("ThrowWeapon", RpcTarget.All);
        isDead = true;
        monsterMove.Stop();

        animator.SetTrigger(hashDead);

        DisableAttackCollider();

        StartCoroutine(Disappear());
    }

    [PunRPC]
    protected void SetState(State _state)
    {
        this.state = _state;
    }

    [PunRPC]
    protected void SetTarget(bool _isTarget)
    {
        isTarget = _isTarget;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(weaponState);

        }
        else
        {
            //this.weaponState = (int)stream.ReceiveNext();

        }
    }
    #endregion

    #region Coroutine
    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(8.0f);

        this.gameObject.SetActive(false);
    }
    #endregion




}
