using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Unity.VisualScripting;

public class ItemTest : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isOn;
    // Start is called before the first frame update
    void Awake()
    {
        isOn = true;    
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.SetActive(isOn);
    }

    public void SetStatus(Vector3 pos)
    {
        this.transform.localPosition = pos;
        this.isOn = !isOn;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isOn);

        }
        else
        {
            this.isOn = (bool)stream.ReceiveNext();

        }
    }
}
