using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Photon.Pun;

public class FollowCam : MonoBehaviourPun
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float distance = 5.0f;
    [SerializeField]
    private float height = 2.0f;
    [SerializeField]
    private float targetOffset = 1.0f;
    [SerializeField]
    private float moveSpeed = 10.0f;
    [SerializeField]
    private float mouseY = 0.0f;
    [SerializeField]
    private float sensitivity = 2.0f;
    [SerializeField]
    private float mouseWheel = 0.0f;

    private Transform cameraTransform;


    private void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        OnStartFollowing();
    }
    private void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.visible = !Cursor.visible;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

        }

        if(Cursor.visible == false)
        {
            FollowMode();
        }
        
    }

    private void FollowMode()
    {



        


        mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        mouseY = Input.GetAxis("Mouse Y");

        distance -= mouseWheel * sensitivity;
        distance = Mathf.Clamp(distance, 1.0f, 5.0f);

        targetOffset += mouseY * sensitivity * distance * 0.2f * Time.deltaTime;
        targetOffset = Mathf.Clamp(targetOffset, 1.0f, 2.5f);

        Vector3 camPos = this.transform.position - (this.gameObject.transform.forward * distance) + (this.gameObject.transform.up * height);

        cameraTransform.position = Vector3.Slerp(cameraTransform.position, camPos, moveSpeed * Time.deltaTime);

        cameraTransform.LookAt(this.transform.position + (this.transform.up * targetOffset));
    }

    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }



}