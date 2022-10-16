using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Pun.Demo.PunBasics;


public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Private Field
    [SerializeField]
    private GameObject beams;
    [SerializeField]
    private GameObject playerUiPrefab;
    private bool IsFiring;
    private bool isGetItem;
    
    #endregion

    #region Public Fields
    public float Health = 1.0f;
    public static GameObject LocalPlayerInstance;
    public List<GameObject> items;
    public GameObject cube;
    #endregion




    #region Monobehaviour Callbacks
    private void Awake()
    {


        if (beams == null)
        {
            Debug.LogError("Beams Reference", this);
        }
        else
        {
            beams.SetActive(false);
        }

        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;

        }

        DontDestroyOnLoad(this.gameObject);


    }

    private void Start()
    {
        PlayerCamera _cameraWork = this.gameObject.GetComponent<PlayerCamera>();
        isGetItem = false;
        items = new List<GameObject>();
        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("Missing CameraWork", this);
        }

        if(playerUiPrefab != null)
        {
            Debug.Log("UI Instantiating");
            GameObject _uiGo = Instantiate(playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

#if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        };
#endif

    }



    private void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }

        if (beams != null && IsFiring != beams.activeInHierarchy)
        {
            beams.SetActive(IsFiring);
        }

        if (Health <= 0f)
        {
            GameManager.Instance.LeaveRoom();
        }


    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("충돌발생");



    }


    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (!other.name.Contains("Beam"))
        {
            return;
        }

        Health -= 0.1f;
    }

    private void OnTriggerStay(Collider other)
    {


        if (!photonView.IsMine)
        {
            return;
        }

        if (other.gameObject.CompareTag("Item"))
        {
            //Debug.Log("ITEM COLLISON");

            GetItem(other.gameObject);
            return;
        }

        if (!other.name.Contains("Beam"))
        {
            return;
        }

        Health -= 0.1f * Time.deltaTime;

        
    }

#if !UNITY_5_4_OR_NEWER
/// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
void OnLevelWasLoaded(int level)
{
    this.CalledOnLevelWasLoaded(level);
}
#endif

    public void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
        if (playerUiPrefab != null)
        {
            Debug.Log("UI Instantiating");
            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }

    #endregion

    #region Public Methods
    
    public void GetItem(GameObject item)
    {
        if(isGetItem == false)
        {
            //Debug.Log("NO Get Item");
            return;
        }
        if(items.Count >= 1)
        {
            //Debug.Log("Inventory Full");
            return;
        }
        //Debug.Log("GetItem");
        items.Add(item);
        PhotonView pv = item.GetPhotonView();
        pv.TransferOwnership(PhotonNetwork.LocalPlayer);
        item.GetComponent<Item>().SetStatus();
        //pv.RPC("SetStatus", RpcTarget.All);
        
        


    }

    public void ThrowItem()
    {
        if (items.Count <= 0)
        {
            //Debug.Log("No Item");
            return;
        }

        PhotonView pv = items[0].GetPhotonView();
        items[0].GetComponentInParent<Item>().SetThrow(this.transform.position + this.transform.forward.normalized + this.transform.up.normalized);
        
        //pv.RPC("SetThrow", RpcTarget.All, (this.transform.position + this.transform.forward.normalized + this.transform.up.normalized));
        items.RemoveAt(0);
        
        
    }

    public bool IsGetItemButton()
    {
        return isGetItem;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(IsFiring);
            stream.SendNext(Health);
        }
        else
        {
            this.IsFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }
    }

    public void ProcessInputs()
    {


        if (Input.GetButtonDown("Fire1"))
        {
            if (!IsFiring)
            {
                IsFiring = true;
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (IsFiring)
            {
                IsFiring = false;
            }
        }

        if (Input.GetKeyDown("g"))
        {
            isGetItem = true;
            Debug.Log("KEY ITEM");
        }

        if (Input.GetKeyUp("g"))
        {
            isGetItem = false;
            Debug.Log("UNKEY ITEM");
        }

        if (Input.GetKeyDown("f"))
        {
            ThrowItem();
        }
    }
    #endregion










}
