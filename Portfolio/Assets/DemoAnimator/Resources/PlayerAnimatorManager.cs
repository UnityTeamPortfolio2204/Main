using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAnimatorManager : MonoBehaviourPun
{
    // Start is called before the first frame update
    private Animator animator;
    [SerializeField]
    private float directionDampTime = 0.25f;
    void Start()
    {
        animator = GetComponent<Animator>();

        if (!animator)
        {
            Debug.LogError("Missing Animator");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!animator)
        {
            return;
        }

        if(photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.IsName("Base Layer.Run"))
        {
            if (Input.GetButtonDown("Fire2"))
            {
                animator.SetTrigger("Jump");
            }
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (v < 0)
        {
            v = 0;
        }

        animator.SetFloat("Speed", h * h + v * v);
        animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
    }
}
