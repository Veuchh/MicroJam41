using System;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPointHandler : MonoBehaviour
{
    const string ANCHOR_POINT_TAG = "AnchorPoint";

    [SerializeField]
    float anchorPointMaxReach = 4;

    [SerializeField]
    PlayerRocket playerRocket;

    [SerializeField]
    DistanceJoint2D playerSpringJoint;

    bool isHoldingAnchorPoint;
    bool isHoldInputDown = false;

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
    }

    private void ReleaseAnchorPoint()
    {
        isHoldingAnchorPoint = false;
        playerSpringJoint.connectedBody = null;
        playerSpringJoint.enabled = false;
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
            playerSpringJoint.connectedBody = validAnchorPoints[0].RB;
            playerSpringJoint.distance = Vector2.Distance(transform.position, validAnchorPoints[0].transform.position);
            playerSpringJoint.enabled = true;
            isHoldingAnchorPoint = true;
            playerRocket.OnAnchorPointGrabbed();
        }

    }
}
