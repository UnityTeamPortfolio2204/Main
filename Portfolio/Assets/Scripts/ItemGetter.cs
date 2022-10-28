using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemGetter : MonoBehaviourPun, IPunObservable
{
    #region SerializeField
    [SerializeField]
    private PlayerControl playerControl;
    #endregion

    #region PrivateField
    private bool isGetKey = false;
    private bool isGettingItem = false;
    #endregion

    #region PublicField
    #endregion

    #region MonobehaviourField
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isGetKey = true;
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            isGetKey = false;
        }

        if (Input.GetKey(KeyCode.F))
        {
            playerControl.ThrowWeapon();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Weapon") == false)
        {
            return;
        }

        if (isGetKey && !isGettingItem)
        {
            isGettingItem = true;
            playerControl.GetWeapon(other.gameObject);
            isGettingItem = false;
        }
    }
    #endregion

    #region PhotonField
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {

        }
    }
    #endregion
    


    #region PlayerControlCode
    /*
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
        weaponState = 0;
        animator.SetInteger("PlayerType", playerType);
        _uiGo.GetComponent<PlayerUI>().SetWeapon(0);
    }
     */
    #endregion
}
