using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HeroControl : MonoBehaviour
{
    [SerializeField]
    private float jogSpeed = 10.0f;
    [SerializeField]
    private float runSpeed = 10.0f;
    [SerializeField]
    private float rotSpeed = 10.0f;

    private float speed = 0.0f;

    private float hValue = 0.0f;
    private float vValue = 0.0f;
    private float mouseX = 0.0f;

    public bool isAttack = false;

    private Vector3 direction;

    private Animator animator;
    private HeroDamaged heroDamaged;

    private readonly int hashAttack = Animator.StringToHash("HeroAttack");

    private HeroAttackCollider attackCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        heroDamaged = GetComponent<HeroDamaged>();
        attackCollider = GetComponentInChildren<HeroAttackCollider>();
    }

    private void Start()
    {
        attackCollider.gameObject.GetComponent<SphereCollider>().enabled = false;
    }

    private void Update()
    {
        hValue = Input.GetAxis("Horizontal");
        vValue = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");

        direction = Vector3.right * hValue + Vector3.forward * vValue;

        if (Input.GetKey(KeyCode.LeftShift))
            speed = runSpeed;
        else
            speed = jogSpeed;

        transform.Translate(direction * speed * Time.deltaTime);

        if (Input.GetMouseButton(1))
        {
            transform.Rotate(Vector3.up * mouseX * rotSpeed * Time.deltaTime);
        }

        animator.SetFloat("Velocity", direction.magnitude);
        animator.SetFloat("Horizontal", hValue);
        animator.SetFloat("Vertical", vValue);

        if (Input.GetMouseButtonDown(0))
            Attack();
    }

    private void Attack()
    {
        if (heroDamaged.isDamaged) return;
        if (isAttack) return;

        isAttack = true;
        animator.SetTrigger(hashAttack);
    }

    private void EnableAttackCollider()
    {
        attackCollider.gameObject.GetComponent<SphereCollider>().enabled = true;
    }

    private void EndAttack()
    {
        isAttack = false;

        attackCollider.gameObject.GetComponent<SphereCollider>().enabled = false;
    }
}
