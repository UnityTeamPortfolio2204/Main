using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPlayer : MonoBehaviour
{
    #region SerializedField
    [SerializeField]
    private Text playerNameText;
    #endregion

    #region PublicMethod
    public void SetName(string value)
    {
        playerNameText.text = value;
    }
    #endregion
}
