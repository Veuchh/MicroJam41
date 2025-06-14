using System;
using System.Collections.Generic;
using Core.Player;
using UnityEngine;

public class AnchorPointHandler : MonoBehaviour
{
    const string ANCHOR_POINT_TAG = "AnchorPoint";
    const string ANIMATOR_BOOL = "IsGrabbing";

    [SerializeField]
    float anchorPointMaxReach = 4;

    [SerializeField]
    float minGrabDistance = 1.5f;

    [SerializeField]
    Animator animator;

    [SerializeField]
    Transform tongueStartPosition;

    [SerializeField]
    LineRenderer tongue;

    [SerializeField]
    SpriteRenderer tongueTip;

    [SerializeField]
    PlayerRocket playerRocket;

    [SerializeField]
    DistanceJoint2D playerSpringJoint;

    AnchorPoint heldAnchorPoint;

    bool isHoldingAnchorPoint;
    bool isHoldInputDown = false;

    public bool IsHoldingAnchorPoint => isHoldingAnchorPoint;
    public AnchorPoint HeldAnchorPoint => heldAnchorPoint;

    private void Awake()
    {
        tongue.transform.SetParent(null);
        tongue.transform.position = Vector3.zero;
    }

    public void OnNewAnchorPointInput(bool isHoldingInput)
    {
        isHoldInputDown = isHoldingInput;

        if (isHoldingAnchorPoint && !isHoldingInput)
            ReleaseAnchorPoint();
    }

    private void Update()
    {
        if (!isHoldingAnchorPoint && isHoldInputDown)
            TryGrabAnchorPoint();

        if (isHoldingAnchorPoint && heldAnchorPoint)
        {
            tongue.SetPosition(0, tongueStartPosition.position);
            tongue.SetPosition(1, heldAnchorPoint.transform.position);
            tongueTip.transform.position = heldAnchorPoint.transform.position;
        }
    }

    private void TryGrabAnchorPoint()
    {
        List<AnchorPoint> validAnchorPoints = new List<AnchorPoint>();

        foreach (var collider in Physics2D.OverlapCircleAll(transform.position, anchorPointMaxReach))
        {
            if (collider.CompareTag(ANCHOR_POINT_TAG) && collider.GetComponent<AnchorPoint>() is AnchorPoint anchorPoint)
                validAnchorPoints.Add(anchorPoint);
        }

        if (validAnchorPoints.Count > 0)
        {
            GrabAnchorPoint(validAnchorPoints[0]);
        }
    }

    private void GrabAnchorPoint(AnchorPoint anchorPoint)
    {
        playerSpringJoint.connectedBody = anchorPoint.RB;
        playerSpringJoint.distance = Mathf.Max(minGrabDistance,
            Vector2.Distance(transform.position, anchorPoint.transform.position));
        playerSpringJoint.enabled = true;
        isHoldingAnchorPoint = true;
        playerRocket.OnAnchorPointGrabbed();
        animator.SetBool(ANIMATOR_BOOL, true);
        heldAnchorPoint = anchorPoint;
        tongue.gameObject.SetActive(true);
        tongueTip.gameObject.SetActive(true);
    }

    private void ReleaseAnchorPoint()
    {
        isHoldingAnchorPoint = false;
        playerSpringJoint.connectedBody = null;
        playerSpringJoint.enabled = false;
        animator.SetBool(ANIMATOR_BOOL, false);
        heldAnchorPoint = null;
        tongue.gameObject.SetActive(false);
        tongueTip.gameObject.SetActive(false);
    }
}
