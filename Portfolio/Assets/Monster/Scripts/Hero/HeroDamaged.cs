using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDamaged : MonoBehaviour
{
    [SerializeField]
    private float maxHp = 500;
    [SerializeField]
    private float curHp = 500;

    private Animator animator;
    private HeroControl heroControl;

    private readonly int hashDamaged = Animator.StringToHash("HeroHitted");

    public bool isDamaged = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        heroControl = GetComponent<HeroControl>();
    }

    public void Damaged(float damage)
    {
        if (heroControl.isAttack) return;
        if (isDamaged) return;

        isDamaged = true;
        animator.SetTrigger(hashDamaged);
        curHp -= damage;
    }

    private void EndDamaged()
    {
        isDamaged = false;
    }
}
