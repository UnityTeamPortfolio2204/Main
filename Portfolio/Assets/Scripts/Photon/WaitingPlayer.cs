using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPlayer : MonoBehaviour
{
    [SerializeField]
    private Text playerNameText;
    // Start is called before the first frame update
    
    public void SetName(string value)
    {
        playerNameText.text = value;
    }
}
