using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackCollider : MonoBehaviour
{
    [SerializeField]
    private float damage = 0.0f;

    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HeroDamaged>().Damaged(damage);
        }
    }
}
