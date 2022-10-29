using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackCollider : MonoBehaviour
{
    [SerializeField]
    private float damage = 0.0f;

    [SerializeField]
    private string particle;

    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Player"))
        {
            Vector3 offSet = new Vector3(0, 1, 0);

            if (!other.GetComponent<PlayerControl>().IsDamaged())
            {
                ParticleManager.instance.Play(particle, other.transform.position + offSet, other.transform.rotation);
            }

            other.GetComponent<PlayerControl>().Damaged(damage);
        }
    }
}
