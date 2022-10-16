using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonster : MonoBehaviour
{
    [SerializeField]
    private float curHP;


    public void Damaged(float damage)
    {
        curHP -= damage;
    }
}
