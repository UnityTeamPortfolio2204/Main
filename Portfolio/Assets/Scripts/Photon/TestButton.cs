using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class TestButton : MonoBehaviour
{
    public void ToMain()
    {
        PhotonNetwork.LeaveRoom();
    }
}
