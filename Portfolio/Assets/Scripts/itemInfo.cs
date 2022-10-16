using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class itemInfo : MonoBehaviourPunCallbacks, IPunObservable
{


    private void Awake()
    {

    }

    [PunRPC]
    public void SetStatus()
    {
        //this.gameObject.GetComponentInParent<Item>().SetStatus();
        this.gameObject.SetActive(false);

    }

    [PunRPC]
    public void SetThrow(Vector3 pos)
    {
        //this.gameObject.GetComponentInParent<Item>().SetThrow(pos);
        this.transform.position = pos;
        this.gameObject.SetActive(true);
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
