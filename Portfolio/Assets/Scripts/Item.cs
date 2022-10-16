using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;

public class Item : MonoBehaviourPunCallbacks, IPunObservable
{


    // Start is called before the first frame update

    private void Awake()
    {

    }

    private void Update()
    {

        
    }
    //[PunRPC]
    public void SetStatus()
    {
        Debug.Log(this.GetComponentInParent<PhotonView>().ViewID);

        this.gameObject.transform.parent = PlayerManager.LocalPlayerInstance.transform;
        this.gameObject.transform.localPosition = new Vector3(1f, 0f, 0f);
    }


    //[PunRPC]
    public void SetThrow(Vector3 pos)
    {
        this.gameObject.transform.parent = null;
        this.transform.position = pos;
        //SetStatus();
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
