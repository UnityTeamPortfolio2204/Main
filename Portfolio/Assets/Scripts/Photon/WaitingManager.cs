using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class WaitingManager : MonoBehaviourPun, IPunObservable
{
    #region SerializedField
    [SerializeField]
    private AudioSource startAudio;
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
    private Text modelText;
    [SerializeField]
    private Text roomName;
    [SerializeField]
    private Transform scrollContent;
    [SerializeField]
    private GameObject waitingPlayer;
    [SerializeField]
    private GameObject _modelSelector;
    #endregion

    #region PrivateField
    private Dictionary<string, GameObject> playerListForScroll = new Dictionary<string, GameObject>();
    WaitForSeconds checkStateTime;
    private bool isCheckPlayer;
    private GameObject modelSelector;
    #endregion




    #region MonobehaviourField
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
        modelText.text = ((int)modelNumber.value).ToString();
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        isCheckPlayer = true;
        checkStateTime = new WaitForSeconds(0.5f);
        StartCoroutine(CheckPlayer());
    }
    

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            SceneManager.LoadScene(0);
        }

        playerCount.text = $"{players.Length}/5";

        switch ((int)modelNumber.value)
        {
            case 0:
                modelText.text = "�����";
                break;
            case 1:
                modelText.text = "�����";
                break;
            case 2:
                modelText.text = "����ũ";
                break;
            case 3:
                modelText.text = "����ũ";
                break;
            case 4:
                modelText.text = "�ذ�";
                break;
        }
        
    }
    #endregion

    #region CoroutineField
    private IEnumerator CheckPlayer()
    {
        while (true)
        {
            yield return checkStateTime;
            if (!isCheckPlayer) yield break;

            players = PhotonNetwork.PlayerList;
            for (int i = 0; i < players.Length; i++)
            {
                Debug.Log(players[i].NickName);
                if (playerListForScroll.ContainsKey(players[i].NickName) == false)
                {
                    GameObject temp = Instantiate(waitingPlayer, scrollContent);
                    temp.GetComponent<WaitingPlayer>().SetName(players[i].NickName);
                    playerListForScroll.Add(players[i].NickName, temp);
                }
            }

            
        }
    }
    #endregion


    #region PublicField
    public void GameStart()
    {
        this.photonView.RPC("SetModelNumber", RpcTarget.All);
        startAudio.Play();
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("Roomfor 1");
        //PhotonNetwork.LoadLevel("MainGame");
    }
    public void LeaveRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            isCheckPlayer = false;
            StopCoroutine(CheckPlayer());
        }
        this.photonView.RPC("DeletePlayerList", RpcTarget.All, modelSelector.GetPhotonView().Owner.NickName);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);

    }

    public int GetModelNumber()
    {
        return (int)modelNumber.value;
    }
    #endregion


    #region PhotonField
    [PunRPC]
    public void DeletePlayerList(string value)
    {
        string test = "remove : " + value;
        Debug.Log(test);
        Destroy(playerListForScroll[value].gameObject);
        playerListForScroll.Remove(value);
    }

    

    [PunRPC]
    public void SetModelNumber()
    {
        modelSelector.GetComponent<ModelNumber>().SetModelNumber((int)modelNumber.value);
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
    #endregion
}
