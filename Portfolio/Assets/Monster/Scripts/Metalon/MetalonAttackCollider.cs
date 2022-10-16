using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalonAttackCollider : MonoBehaviour
{
    [SerializeField]
    private float damage = 10.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerControl>().Damaged(damage);
        }
    }
}
