using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class PlayerUI : MonoBehaviour
{
    #region Priavate Fields
    [SerializeField]
    private Text playerNameText;
    [SerializeField]
    private Image healthBar;
    //private Slider playerHealthSlider;
    [SerializeField]
    private Text currentHealthText;
    [SerializeField]
    private Text maxHealthText;
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);


    #endregion

    #region Private Fields Messages
    private PlayerControl target;
    private float characterControllerHeight = 0f;
    private Transform targetTransform;
    private Vector3 targetPosition;

    private float currentHealth;
    private float maxHealth;

    #endregion


    #region MonoBehaviour Callbacks

    private void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);


    }
    private void Update()
    {
        
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        if (!target.photonView.IsMine)
        {
            this.gameObject.SetActive(false);
            return;
        }

        if(healthBar != null)
        {
            currentHealth = target.GetHp();
            currentHealthText.text = currentHealth.ToString();
            healthBar.fillAmount = currentHealth/maxHealth;
            
            
        }
    }
    #endregion

    #region Public Methods


    public void SetTarget(PlayerControl _target)
    {
        if(_target == null)
        {
            Debug.LogError("Missing PlayerManager", this);
            return;
        }

        Debug.Log("UI Create");
        target = _target;
        
        

        CharacterController _characterController = _target.GetComponent<CharacterController>();
        if(_characterController != null)
        {
            characterControllerHeight = _characterController.height;
        }

        if(playerNameText != null)
        {
            playerNameText.text = target.photonView.Owner.NickName;
        }

        maxHealth = target.GetMaxHp();
        currentHealth = maxHealth;
        maxHealthText.text = maxHealth.ToString();
    }

    public void LateUpdate()
    {
     /*   if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }*/
    }

    public Text GetName()
    {
        return playerNameText;
    }
    #endregion


}
