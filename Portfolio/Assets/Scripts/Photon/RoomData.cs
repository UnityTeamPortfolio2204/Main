
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomData : MonoBehaviour
{
    

    #region Private Fields

    private Text RoomInfoText;
    private RoomInfo _roomInfo;

    #endregion

    #region Public Fields
    public InputField userId;
    #endregion

    #region Class Fields
    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            RoomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
          
        }
    }
    #endregion

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        RoomInfoText = GetComponentInChildren<Text>();
        userId = GameObject.Find("InputID").GetComponent<InputField>();
    }

    private void Update()
    {
        if (!this.RoomInfo.IsOpen)
        {
            this.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Public Methods
    public void OnEnterRoom(string roomName)
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 5;

        PhotonNetwork.NickName = userId.text;
        PhotonNetwork.JoinOrCreateRoom(_roomInfo.Name, ro, TypedLobby.Default);
    }
    #endregion
}
