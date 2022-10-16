using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    #region Private Fields

    private readonly string gameVersion = "v1.0";
    private string userId = "0jui";

    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();

    #endregion

    #region Public Fields
    public InputField userIdText;
    public InputField roomNameText;

    public GameObject roomPrefab;
    public Transform scrollContent;
    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        //방장이 혼자 씬을 로딩하면 나머지 사람들은 자동으로 싱크
        PhotonNetwork.AutomaticallySyncScene = true;
        //게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;
        //서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Start()
    {
        Debug.Log("0. 포톤 매니저 시작");
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
        userIdText.text = userId;
        PhotonNetwork.NickName = userId;
    }
    #endregion

    #region Photon Methods
    public override void OnConnectedToMaster()
    {
        Debug.Log("1. 포톤 서버에 접속");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("2. 로비에 접속");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("랜덤 룸 접속 실패");

        // 룸 속성 설정
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 2;

        roomNameText.text = $"Room_{Random.Range(1, 100):000}";

        // 룸을 생성 > 자동 입장됨
        PhotonNetwork.CreateRoom("room_1", ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("3. 방 생성 완료");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("4. 방 입장 완료");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("WaitingRoom");
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach(var room in roomList)
        {
            if(room.RemovedFromList == true)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    GameObject _room = Instantiate(roomPrefab, scrollContent);
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    roomDict.Add(room.Name, _room);
                }
                else
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }
    }
    #endregion

    #region UI_Button_callback
    public void OnRandomBtn()
    {
        if (string.IsNullOrEmpty(userIdText.text))
        {
            userId = $"USER_{Random.Range(0, 100):00}";
            userIdText.text = userId;
        }

        PlayerPrefs.SetString("USER_ID", userIdText.text);
        PhotonNetwork.NickName = userIdText.text;
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 5;

        if (string.IsNullOrEmpty(roomNameText.text))
        {
            roomNameText.text = $"ROOM_{Random.Range(1, 100):000}";
        }
        PhotonNetwork.NickName = userIdText.text;
        PhotonNetwork.CreateRoom(roomNameText.text, ro);
    }
    #endregion
}
