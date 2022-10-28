using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class PlayerUI : MonoBehaviour
{
    #region Serialized Fields
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
    [SerializeField]
    private Image weaponImage;
    [SerializeField]
    private List<Sprite> weaponImages = new List<Sprite>();


    #endregion

    #region Private Fields
    private PlayerControl target;


    private float currentHealth;
    private float maxHealth;

    private int weaponType = 0;//0. 맨손, 1. 한손검  2. 두손검 3. 도끼

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
        

        if(playerNameText != null)
        {
            playerNameText.text = target.photonView.Owner.NickName;
        }

        maxHealth = target.GetMaxHp();
        currentHealth = maxHealth;
        maxHealthText.text = maxHealth.ToString();
    }


    public void SetWeapon(int weapon)
    {
        weaponImage.sprite = weaponImages[weapon];

    }

    public Text GetName()
    {
        return playerNameText;
    }
    #endregion


}
