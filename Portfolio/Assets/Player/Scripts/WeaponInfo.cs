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

	void Start()
	{
		targetMonsters = new GameObject[20];
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
                target.gameObject.GetComponent<MonsterAI>().Damaged(offensePower * skillCoefficient); // ���� ��ũ��Ʈ�� ��ü

				// ���� �� ����Ʈ


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
