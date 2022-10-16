using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevButton : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            this.gameObject.SetActive(false);
        }
    }


}
