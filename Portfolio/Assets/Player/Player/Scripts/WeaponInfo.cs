using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponInfo : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private float offensePower; // ���� ���ݷ�

    private float skillCoefficient = 1;

    [SerializeField]
    private GameObject[] targetMonsters; // �ߺ� ������ ������, ������ ���� ���͵��� �迭
    private int count = 0; // ������ ���� ���͵��� ����

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
                //target.gameObject.GetComponent<MonsterAI>().Damaged(offensePower * skillCoefficient); // ���� ��ũ��Ʈ�� ��ü

                Vector3 offset = new Vector3(0, 2, 0);

                ParticleManager.instance.Play(particleName, target.transform.position + offset, target.transform.rotation);
                // ����
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
