using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemThrower : MonoBehaviourPun, IPunObservable
{
    #region Serialize Field
    [SerializeField]
    private GameObject dropWeapon;
    #endregion


    #region Private Region
    #endregion
    #region Monobehaviour Callbacks
    private void Awake()
    {
        Debug.Log("Monster's Item Generating");
      

        //    Resources.Load<GameObject>("Particles/" + key);
        
        
    }

    private void Start()
    {
        this.photonView.RPC("SetState", RpcTarget.All, false);
    }
    #endregion



    #region Public Methods
    [PunRPC]
    public void SetState(bool value)
    {
        dropWeapon.SetActive(value);
    }
    [PunRPC]
    public void ThrowWeapon()
    {
        Debug.Log("Monster Throw Item");
        dropWeapon.transform.position = this.transform.position + this.transform.forward * 3;
        dropWeapon.SetActive(true);
    }
    #endregion

    #region PhotonField
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
     /*       stream.SendNext((bool)dropWeapon.activeSelf);*/
        }
        else
        {
           /* bool temp = (bool)stream.ReceiveNext();
            dropWeapon.SetActive(temp);*/
        }
    }
    #endregion
}
