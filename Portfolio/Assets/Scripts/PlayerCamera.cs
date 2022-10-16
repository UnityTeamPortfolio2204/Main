using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCamera : MonoBehaviourPun
{
    #region Private Fields
    [SerializeField]
    private float distance = 7.0f;
    [SerializeField]
    private float height = 3.0f;
    [SerializeField]
    private Vector3 centerOffset = Vector3.zero;
    [SerializeField]
    private bool followOnStart = false;
    [SerializeField]
    private float smoothSpeed = 0.125f;

    Transform cameraTransform;
    bool isFollowing;
    Vector3 cameraOffset = Vector3.zero;
    #endregion

    #region Monobehaviour Callbacks
    private void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (followOnStart)
        {
            OnStartFollowing();
        }
    }

    private void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (cameraTransform == null && isFollowing)
        {
            OnStartFollowing();
        }

        if (isFollowing)
        {
            Cut();
        }
    }
    #endregion

    #region Public Methods
    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;

        Cut();
    }
    #endregion

    #region Private Methods
    private void Follow()
    {
        cameraOffset.z = -distance;
        cameraOffset.y = height;

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.transform.position + this.transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);
        cameraTransform.LookAt(this.transform.position + centerOffset);
    }

    private void Cut()
    {
        cameraOffset.z = -distance;
        cameraOffset.y = height;

        cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);
        cameraTransform.LookAt(this.transform.position + centerOffset);
    }
    #endregion
}
