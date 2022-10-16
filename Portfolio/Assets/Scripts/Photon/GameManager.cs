using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private Button victory;
    [SerializeField]
    private Button defeat;


    public static GameManager Instance;
    public GameObject playerPrefab;
    public GameObject Item;
    private enum GameState
    {
        Playing, Victory, Defeat
    }

    private GameState gameState = GameState.Playing;

    private void Start()
    {
        Instance = this;

        if(playerPrefab == null)
        {
            Debug.LogError("Missing PlayerPrefab");
        }
        else
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);

            if(PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
            
        }

        PhotonNetwork.InstantiateRoomObject(Item.name, new Vector3(0f, 2f, 0f), Quaternion.identity, 0);
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Playing:
                break;
            case GameState.Victory:
                victory.gameObject.SetActive(true);
                break;
            case GameState.Defeat:
                defeat.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Not Master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        //PhotonNetwork.LoadLevel("Roomfor 1");
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
        }

        LoadArena();

    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            LoadArena();
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Victory()
    {
        gameState = GameState.Victory;
    }

    public void Defeat()
    {
        gameState = GameState.Defeat;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameState);
        }
        else
        {
            this.gameState = (GameState)stream.ReceiveNext();
        }
    }


}
