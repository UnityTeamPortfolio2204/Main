using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAttackCollider : MonoBehaviour
{
    [SerializeField]
    private float damage = 50.0f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if(other.CompareTag("Monster"))
        {
            other.GetComponent<MonsterAI>().Damaged(damage);
        }
    }
}
