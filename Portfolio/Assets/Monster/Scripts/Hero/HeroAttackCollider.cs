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
            Vector3 offSet = new Vector3(0, 1, 0);

            ParticleManager.instance.Play(particle, transform.position + transform.forward * 1.5f + offSet, transform.rotation);
         
            other.GetComponentInParent<MonsterAI>().Damaged(damage);
        }
    }
}
