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
    [SerializeField]
    private Slider modelNumber;



    [SerializeField]
    private GameObject _modelSelector;

    private GameObject modelSelector;
    private void Start()
    {

        players = PhotonNetwork.PlayerList;
        if (!PhotonNetwork.IsMasterClient)
        {
            gameStart.gameObject.SetActive(false);
        }

        if (ModelNumber.LocalModelNumber == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            modelSelector = PhotonNetwork.Instantiate(this._modelSelector.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
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
        modelSelector.GetComponent<ModelNumber>().SetModelNumber((int)modelNumber.value);
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("Roomfor 1");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);

    }

    public int GetModelNumber()
    {
        return (int)modelNumber.value;
    }
}
