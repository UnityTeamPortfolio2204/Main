using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaitingManager : MonoBehaviourPun
{
    private Player[] players;
    [SerializeField]
    private Button gameStart;
    [SerializeField]
    private Button leave;
    [SerializeField]
    private Text playerCount;
    private void Start()
    {

        players = PhotonNetwork.PlayerList;
        if (!PhotonNetwork.IsMasterClient)
        {
            gameStart.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            SceneManager.LoadScene(0);
        }
        players = PhotonNetwork.PlayerList;
        playerCount.text = $"{players.Length}/5";
    }

    public void GameStart()
    {
        PhotonNetwork.LoadLevel("Roomfor 1");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);

    }
}
