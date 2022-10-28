using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAttackCollider : MonoBehaviour
{
    [SerializeField]
    private float damage = 50.0f;

    [SerializeField]
    private string particle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if(other.CompareTag("Monster"))
        {
            Vector3 offSet = new Vector3(0, 2, 0);

            ParticleManager.instance.Play(particle, other.transform.position + offSet, other.transform.rotation);
         
            other.GetComponentInParent<MonsterAI>().Damaged(damage);
        }
    }
}
