using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelNumber : MonoBehaviourPun
{
    private int modelNumber = 0;
    public static GameObject LocalModelNumber;
    public int test;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            ModelNumber.LocalModelNumber = this.gameObject;

        }
        DontDestroyOnLoad(this.gameObject);
        
    }

    public void SetModelNumber(int _value)
    {
        modelNumber = _value;
        test = _value;
    }
    
    public int GetModelNumber()
    {
        return modelNumber;
    }

    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }
}
