using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FollowCam1 : MonoBehaviour
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

    private void LateUpdate()
    {
        FollowMode();
    }

    private void FollowMode()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


        mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        mouseY = Input.GetAxis("Mouse Y");

        distance -= mouseWheel * sensitivity;
        distance = Mathf.Clamp(distance, 1.0f, 5.0f);

        targetOffset += mouseY * sensitivity * distance * 0.2f * Time.deltaTime;
        targetOffset = Mathf.Clamp(targetOffset, 1.0f, 2.5f);

        Vector3 camPos = target.position - (target.forward * distance) + (target.up * height);

        transform.position = Vector3.Slerp(transform.position, camPos, moveSpeed * Time.deltaTime);

        transform.LookAt(target.position + (target.up * targetOffset));
    }

}