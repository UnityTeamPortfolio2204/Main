using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class itemInfo : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private int itemCode;


    public int GetCode()
    {
        return itemCode;
    }

    [PunRPC]
    public void PSetActive(bool value)
    {
        this.gameObject.SetActive(value);
    }

    [PunRPC]
    public void PSetPos(Vector3 pos)
    {
        this.transform.position = pos;
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
