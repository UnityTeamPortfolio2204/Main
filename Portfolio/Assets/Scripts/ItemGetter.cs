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

        if (isGetKey)
        {
            playerControl.GetWeapon(other.gameObject);
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
    // Start is called before the first frame update
    

    
}
