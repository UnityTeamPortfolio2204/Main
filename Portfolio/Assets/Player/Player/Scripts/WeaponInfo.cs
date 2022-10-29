using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponInfo : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private float offensePower; // 무기 공격력

    private float skillCoefficient = 1;

    [SerializeField]
    private GameObject[] targetMonsters; // 중복 데미지 방지용, 데미지 입힌 몬스터들의 배열
    private int count = 0; // 데미지 입힌 몬스터들의 숫자

    [SerializeField]
    private TrailRenderer trailRenderer;

    public bool isAttacking = false;

    [SerializeField]
    private string particleName;

    [SerializeField]
    private SoundKey hitsoundKey;

    void Start()
    {
        targetMonsters = new GameObject[20];
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    private void OnTriggerEnter(Collider target)
    {
        if (target.gameObject.CompareTag("Monster"))
        {
            /*PhotonView pv = this.gameObject.GetPhotonView();
            pv.RPC("CheckTargetDamaged", RpcTarget.All, target.gameObject);*/
            CheckTargetDamaged(target.gameObject);


        }
    }


    private void CheckTargetDamaged(GameObject target)
    {
        foreach (GameObject targetMonster in targetMonsters)
        {
            if (targetMonster == target)
            {
                return;
            }
        }

        while (true)
        {
            if (targetMonsters[count] == null)
            {
                targetMonsters[count] = target;
                target.transform.root.gameObject.GetComponent<MonsterAI>().Damaged(offensePower * skillCoefficient);
                //target.gameObject.GetComponent<MonsterAI>().Damaged(offensePower * skillCoefficient); // 몬스터 스크립트로 교체

                Vector3 offset = new Vector3(0, 2, 0);

                ParticleManager.instance.Play(particleName, target.transform.position + offset, target.transform.rotation);
                // 사운드
                //SoundManager.instance.PlaySFX(hitsoundKey, transform.position);

                return;
            }
            else
            {
                count++;
            }

        }
    }

    public void ResetTargets()
    {
        for (int i = 0; i < targetMonsters.Length; i++)
        {
            targetMonsters[i] = null;
        }
    }

    public void SetCoefficient(float value)
    {
        skillCoefficient = value;
    }

    public void EmitTrailRenderer()
    {
        StartCoroutine(EmitCoroutine());
    }

    IEnumerator EmitCoroutine()
    {
        trailRenderer.transform.position = transform.position;

        trailRenderer.emitting = true;



        while (isAttacking)
        {
            Vector3 vector = transform.position;
            vector.y += 0.5f;

            trailRenderer.transform.position = vector;
            yield return new WaitForSeconds(0.1f);
        }

        //yield return new WaitUntil(() => !isAttacking);
        trailRenderer.emitting = false;

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {
        }
    }
}
