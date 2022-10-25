using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using static PlayerControl;

public class PlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private GameObject[] models;

    [SerializeField]
    private int selectedModelNum;

    [SerializeField]
    private float walkSpeed = 10.0f;

    [SerializeField]
    private float runSpeed = 20.0f;

    [SerializeField]
    private float rotSpeed = 100.0f;


    [SerializeField]
    private GameObject playerUiPrefab;
    public static GameObject LocalPlayerInstance;


    [SerializeField]
    private int weaponState = 0;
    [SerializeField]
    private int playerType = 0;

    private GameObject currentWeapon = null;
    private GameObject _uiGo;

    private float hValue = 0.0f;
    private float vValue = 0.0f;
    private float mouseX = 0.0f;
    private float speed = 0.0f;
    private bool isStop = false;

    private bool isRolling = false;
    private bool isDamaged = false;
    private bool isinvincibility = false;

    private Vector3 direction;

    private Animator animator;

    private Rigidbody _rigidbody;

    private readonly int hashMoveSpeed = Animator.StringToHash("PlayerMoveSpeed");
    private readonly int hashType = Animator.StringToHash("PlayerType");

    private bool isAttacking = false;
    private bool isComboEnable = false;
    private bool isComboAttack = false;

    [SerializeField]
    private int attackCount = 0;

    [SerializeField]
    private int curSkillNum = 0;

    [SerializeField]
    private float maxHp = 500;
    [SerializeField]
    private float curHp = 500;


    public enum MotionState
    {
        ONE_HAND_SWORD,
        TWO_HAND_SWORD,
        ONE_HAND_AXE
    }

    [SerializeField]
    private MotionState motionState = MotionState.ONE_HAND_SWORD;

    /*[SerializeField]
	private RuntimeAnimatorController oneHandController;
	[SerializeField]
	private RuntimeAnimatorController twoHandController;*/

    [SerializeField]
    private GameObject rightHandEquip;
    [SerializeField]
    private GameObject leftHandEquip;

    [SerializeField]
    private GameObject[] rightWeapons;
    [SerializeField]
    private GameObject[] leftWeapons;

    [SerializeField]
    private GameObject rightWeapon;
    [SerializeField]
    private GameObject leftWeapon;
    [SerializeField]
    private BoxCollider rightWeaponCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;

        }

        this.selectedModelNum = ModelNumber.LocalModelNumber.GetComponent<ModelNumber>().GetModelNumber();
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        FollowCam _cameraWork = this.gameObject.GetComponent<FollowCam>();
        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("Missing CameraWork", this);
        }

        if (playerUiPrefab != null)
        {
            Debug.Log("UI Instantiating");
            _uiGo = Instantiate(playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

        models[selectedModelNum].SetActive(true);
        //SetAnimatorController(oneHandController);

#if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        };
#endif
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Alpha1))//한손검
        {
            weaponState = 1;
            /*			ChangeWeapon(rightHandEquip, rightWeapons[0], MotionState.ONE_HAND_SWORD);
                        SetAnimatorController(oneHandController);*/
            this.ChangeWeapon(rightHandEquip, rightWeapons[0], MotionState.ONE_HAND_SWORD);
            //this.SetAnimatorController(oneHandController);
            playerType = 0;
            animator.SetInteger("PlayerType", playerType);
        }

        if (Input.GetKey(KeyCode.Alpha2))//두손검
        {
            weaponState = 2;
            /*			ChangeWeapon(rightHandEquip, rightWeapons[1], MotionState.TWO_HAND_SWORD);
                        SetAnimatorController(twoHandController);*/
            this.ChangeWeapon(rightHandEquip, rightWeapons[1], MotionState.TWO_HAND_SWORD);
            //this.SetAnimatorController(twoHandController);
            playerType = 1;
            animator.SetInteger("PlayerType", playerType);
        }

        if (Input.GetKey(KeyCode.Alpha3))//방패
        {
            weaponState = 3;
            /*			ChangeWeapon(leftHandEquip, leftWeapons[0], MotionState.ONE_HAND_SWORD);
                        SetAnimatorController(oneHandController);*/
            this.ChangeWeapon(leftHandEquip, leftWeapons[0], MotionState.ONE_HAND_SWORD);
            //this.SetAnimatorController(oneHandController);
            playerType = 0;
            animator.SetInteger("PlayerType", playerType);
        }

        if (Input.GetKey(KeyCode.Alpha4))//도끼
        {
            weaponState = 4;
            /*			ChangeWeapon(leftHandEquip, leftWeapons[0], MotionState.ONE_HAND_SWORD);
                        SetAnimatorController(oneHandController);*/
            this.ChangeWeapon(rightHandEquip, rightWeapons[2], MotionState.ONE_HAND_AXE);
            //this.SetAnimatorController(oneHandController);
            playerType = 2;
            animator.SetInteger("PlayerType", playerType);
        }


        if (Input.GetMouseButtonDown(0))
        {
            curSkillNum = 0;
            Attack(curSkillNum);
        }

        if (Input.GetMouseButtonDown(1))
        {
            curSkillNum = 1;
            Attack(curSkillNum);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollFront();
        }

        //모션 애니메이터 컨트롤러, 무기체인지 Test용




        Control();
        Move();
    }

#if !UNITY_5_4_OR_NEWER
/// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
void OnLevelWasLoaded(int level)
{
    this.CalledOnLevelWasLoaded(level);
}
#endif

    public void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
        if (playerUiPrefab != null)
        {
            Debug.Log("UI Instantiating");
            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }

    private void RollFront()
    {
        if (isAttacking) return;
        if (isRolling) return;
        if (isDamaged) return;

        animator.SetTrigger("PlayerRoll");
    }

    private void StartRoll()
    {
        isRolling = true;
        isinvincibility = true;
    }
    private void EndRoll()
    {
        isRolling = false;
        isinvincibility = false;
    }

    private void Control()
    {
        if (isStop) return;

        hValue = Input.GetAxis("Horizontal");
        vValue = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        direction = Vector3.right * hValue + Vector3.forward * vValue;

        if (Input.GetKey(KeyCode.LeftShift))
            speed = runSpeed;
        else
            speed = walkSpeed;
    }

    private void Move()
    {
        if (isStop) return;
        if (isRolling || isAttacking) return;
        if (isDamaged) return;

        transform.Translate(direction * speed * Time.deltaTime);
        transform.Rotate(Vector3.up * mouseX * rotSpeed * Time.deltaTime);

        animator.SetFloat("PlayerHorizontal", hValue);
        animator.SetFloat("PlayerVertical", vValue);
        animator.SetFloat(hashMoveSpeed, speed);
    }

    private void Attack(int skillNum)
    {
        if (isRolling) return;
        if (isDamaged) return;

        if (skillNum == 0)
            if (isComboEnable)
            {
                isComboAttack = true;
                return;
            }

        if (isAttacking) return;



        animator.SetTrigger("PlayerAttack");
        animator.SetInteger("PlayerSkill", skillNum);
        if (skillNum == 0)
            animator.SetInteger("PlayerAttackCount", attackCount++);



        isStop = true;
        isAttacking = true;
        speed = 0;
    }

    private void EndAttack()
    {
        isAttacking = false;
        attackCount = 0;

        isStop = false;

        if (isDamaged)
            isDamaged = false;
    }

    private void MeleeAttackColliderEnable()
    {
        if (rightWeaponCollider != null)
            rightWeaponCollider.enabled = true;

        if (rightWeapon != null)
            rightWeapon.GetComponent<WeaponInfo>().SetCoefficient((attackCount + 1) * (curSkillNum + 1));
    }

    private void MeleeAttackColliderDisable()
    {
        if (rightWeaponCollider != null)
        {
            rightWeaponCollider.enabled = false;
            rightWeapon.GetComponent<WeaponInfo>().ResetTargets();
            rightWeapon.GetComponent<WeaponInfo>().SetCoefficient(1);
        }
    }

    private void StartComboEnable()
    {
        isComboEnable = true;
    }

    private void EndComboEnable()
    {
        isComboEnable = false;
    }

    private void ComboAttack()
    {
        if (!isComboAttack) return;

        isComboAttack = false;

        animator.SetTrigger("PlayerAttack");


        animator.SetInteger("PlayerAttackCount", attackCount++);

    }

    private void SetAnimatorController(RuntimeAnimatorController controller)
    {
        if (isAttacking) return;
        if (animator.runtimeAnimatorController == controller) return;

        animator.runtimeAnimatorController = controller;
    }

    private void ChangeWeapon(GameObject hand, GameObject weapon, MotionState state)
    {
        if (isAttacking) return;
        if(hand == null)
        {
            this.leftWeapon = null;
            foreach (GameObject leftWeapon in leftWeapons)
            {
                leftWeapon.SetActive(false);
                

            }
            this.rightWeapon = null;
            foreach (GameObject rightWeapon in rightWeapons)
            {
                rightWeapon.SetActive(false);
            }

            motionState = state;

        }

        if (hand == rightHandEquip || (hand == leftHandEquip && motionState == MotionState.TWO_HAND_SWORD))
            foreach (GameObject rightWeapon in rightWeapons)
            {
                rightWeapon.SetActive(false);
                this.rightWeapon = null;
                rightWeaponCollider = null;
            }

        if (hand == leftHandEquip || state == MotionState.TWO_HAND_SWORD)
        {
            foreach (GameObject leftWeapon in leftWeapons)
            {
                leftWeapon.SetActive(false);
                this.leftWeapon = null;
            }
        }

        foreach (GameObject rightWeapon in rightWeapons)
        {
            if (weapon == rightWeapon)
            {
                weapon.SetActive(true);
                this.rightWeapon = rightWeapon;
                rightWeaponCollider = rightWeapon.GetComponent<BoxCollider>();
                motionState = state;
            }
        }

        foreach (GameObject leftWeapon in leftWeapons)
        {
            if (weapon == leftWeapon)
            {
                weapon.SetActive(true);
                this.leftWeapon = leftWeapon;
                motionState = state;
            }
        }
    }

    public void Damaged(float damage)
    {
        if (isDamaged) return;
        if (isinvincibility) return;

        isDamaged = true;
        if (!isAttacking)
            animator.SetTrigger("PlayerHitted");
        curHp -= damage;
    }

    private void EndDamaged()
    {
        isDamaged = false;
    }

    public float GetHp()
    {
        return curHp;
    }

    public float GetMaxHp()
    {
        return maxHp;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(weaponState);
            stream.SendNext(selectedModelNum);
        }
        else
        {
            this.weaponState = (int)stream.ReceiveNext();
            if(this.weaponState == 0)
            {
                this.ChangeWeapon(null, null, MotionState.ONE_HAND_SWORD);
                playerType = 0;
                animator.SetInteger("PlayerType", playerType);
                
            }

            if (this.weaponState == 1)//한손검
            {
                /*			ChangeWeapon(rightHandEquip, rightWeapons[0], MotionState.ONE_HAND_SWORD);
                            SetAnimatorController(oneHandController);*/
                this.ChangeWeapon(rightHandEquip, rightWeapons[0], MotionState.ONE_HAND_SWORD);
                //this.SetAnimatorController(oneHandController);
                animator.SetInteger("PlayerType", 0);
            }

            if (this.weaponState == 2)//두손검
            {
                /*			ChangeWeapon(rightHandEquip, rightWeapons[1], MotionState.TWO_HAND_SWORD);
                            SetAnimatorController(twoHandController);*/
                this.ChangeWeapon(rightHandEquip, rightWeapons[1], MotionState.TWO_HAND_SWORD);
                //this.SetAnimatorController(twoHandController);
                animator.SetInteger("PlayerType", 1);
            }

            if (this.weaponState == 3)//방패
            {
                /*			ChangeWeapon(leftHandEquip, leftWeapons[0], MotionState.ONE_HAND_SWORD);
                            SetAnimatorController(oneHandController);*/
                this.ChangeWeapon(leftHandEquip, leftWeapons[0], MotionState.ONE_HAND_SWORD);
                //this.SetAnimatorController(oneHandController);
                animator.SetInteger("PlayerType", 0);
            }
            if (this.weaponState == 4)//도끼
            {
                /*			ChangeWeapon(leftHandEquip, leftWeapons[0], MotionState.ONE_HAND_SWORD);
                            SetAnimatorController(oneHandController);*/
                this.ChangeWeapon(leftHandEquip, leftWeapons[0], MotionState.ONE_HAND_AXE);
                //this.SetAnimatorController(oneHandController);
                animator.SetInteger("PlayerType", 2);
            }

            models[this.selectedModelNum].SetActive(false);
            this.selectedModelNum = (int)stream.ReceiveNext();
            models[this.selectedModelNum].SetActive(true);
        }
    }

    public void GetWeapon(GameObject _weapon)
    {
        ThrowWeapon();
        currentWeapon = _weapon;
        weaponState = currentWeapon.GetComponent<itemInfo>().GetCode();


        this.ChangeWeapon(rightHandEquip, rightWeapons[weaponState - 1], MotionState.TWO_HAND_SWORD);

        playerType = weaponState - 1;
        _uiGo.GetComponent<PlayerUI>().SetWeapon(weaponState);
        animator.SetInteger("PlayerType", playerType);

        PhotonView pv = currentWeapon.GetPhotonView();
        pv.RPC("PSetActive", RpcTarget.All, false);
        
        
    }

    public void ThrowWeapon()
    {
        if(currentWeapon == null)
        {
            return;
        }

        
        PhotonView pv = currentWeapon.GetPhotonView();
        pv.RPC("PSetActive", RpcTarget.All, true);
        pv.RPC("PSetPos", RpcTarget.All, (this.transform.position + this.transform.forward));
        currentWeapon = null;
        ChangeWeapon(null, null, MotionState.ONE_HAND_SWORD);
        playerType = 0;
        animator.SetInteger("PlayerType", playerType);
        _uiGo.GetComponent<PlayerUI>().SetWeapon(0);
    }
}
