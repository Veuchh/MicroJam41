using System;
using UnityEngine;

public class PlayerRocket : MonoBehaviour {
    private static readonly int _ANIMATOR_PARAM_AMMO_COUNT = Animator.StringToHash("AmmoCount");
    private static readonly int _ANIMATOR_PARAM_FIRE_TRIGGER = Animator.StringToHash("Fire");

    public delegate void RocketFireEvent(Vector2 pos, Vector2 dir);
    public static event RocketFireEvent onRocketFired;
    
    [SerializeField] Rigidbody2D rb;
    [Header("Rocket Refill")]
    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] float rocketRefillRaycastDist;
    [SerializeField] int maxRocketAmount;
    [SerializeField] int MinRocketAmountOnGrabAnchorPoint = 1;
    [SerializeField] float rocketCooldown = .1f;
    [Header("Rocket Inertia")]
    [SerializeField] float slingshotStrengthMultiplier = 1;
    [SerializeField] float rocketStrength;
    [SerializeField] float baseMomentumMultiplierOnFireIfSameDir = .5f;
    [SerializeField] float momentumMultiplierOnFireSameDirection = .25f;
    [SerializeField] float momentumMultiplierOnFireOppositeDirection = .1f;
    [Header("GFX")]
    [SerializeField] Transform rocketLauncher;
    [SerializeField] Animator rocketLauncherAnimator;
    [SerializeField] SpriteRenderer rocketLauncherSprite;
    [SerializeField] Transform eyeLeft;
    [SerializeField] Transform eyeRight;

    AnchorPointHandler anchorPointHandler;
    int currentRocketAmount;
    float nextAllowedRocketShot;
    Vector2 currentRocketLauncherDirection;

    private void Start()
    {
        anchorPointHandler = GetComponent<AnchorPointHandler>();
        RefillRockets();
    }

    private void Update()
    {
        float rocketLauncherRotation = Vector2.SignedAngle(Vector2.up, currentRocketLauncherDirection) - 90;
        rocketLauncher.rotation = Quaternion.Euler(0, 0, rocketLauncherRotation);

        rocketLauncherSprite.flipY = rocketLauncherRotation < -90 || rocketLauncherRotation > 90;

        eyeLeft.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, currentRocketLauncherDirection));
        eyeRight.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, currentRocketLauncherDirection));

        if (rb.linearVelocityY <= 0 && Physics2D.Raycast(transform.position, Vector2.down, rocketRefillRaycastDist, platformLayerMask))
        {
            RefillRockets();
        }
    }

    public void RefillRockets()
    {
        currentRocketAmount = maxRocketAmount;
        rocketLauncherAnimator.SetInteger(_ANIMATOR_PARAM_AMMO_COUNT, currentRocketAmount);

        GameCanvas.Instance?.UpdateRocketsUI(currentRocketAmount);
    }

    public void UpdateRocketLauncherDirection(Vector2 newDirection)
    {
        currentRocketLauncherDirection = newDirection.normalized;
    }

    public void OnFireClicked()
    {
        if (currentRocketAmount <= 0 || Time.time < nextAllowedRocketShot)
            return;


        FireRocket();
        onRocketFired?.Invoke(transform.position, currentRocketLauncherDirection);
        rocketLauncherAnimator.SetInteger(_ANIMATOR_PARAM_AMMO_COUNT, currentRocketAmount);
        rocketLauncherAnimator.SetTrigger(_ANIMATOR_PARAM_FIRE_TRIGGER);
    }

    private void FireRocket()
    {
        nextAllowedRocketShot = Time.time + rocketCooldown;
        currentRocketAmount--;

        GameCanvas.Instance?.UpdateRocketsUI(currentRocketAmount);

        float dotProduct = Vector2.Dot(rb.linearVelocity, -currentRocketLauncherDirection);

        if (dotProduct > 0)
            rb.linearVelocity *= (1 / dotProduct) * momentumMultiplierOnFireSameDirection + baseMomentumMultiplierOnFireIfSameDir;
        else
            rb.linearVelocity *= momentumMultiplierOnFireOppositeDirection;

        //Slingshotting when shooting perpendicularly to held anchor
        float slingShotMultiplier = 1;
        if (anchorPointHandler.IsHoldingAnchorPoint)
        {
            Vector2 transformPositionVector2 = new Vector2(transform.position.x, transform.position.y);
            Vector2 anchorPositionVector2 = new Vector2(anchorPointHandler.HeldAnchorPoint.transform.position.x, anchorPointHandler.HeldAnchorPoint.transform.position.y);
            Vector2 direction = transformPositionVector2 - anchorPositionVector2;
            float anchorToAimDotProduct = Vector2.Dot(-currentRocketLauncherDirection.normalized, direction.normalized);

            slingShotMultiplier += (- (Mathf.Abs(anchorToAimDotProduct) - 1)) * slingshotStrengthMultiplier;
        }

        rb.AddForce(-currentRocketLauncherDirection * rocketStrength * slingShotMultiplier, ForceMode2D.Impulse);
    }

    public void OnAnchorPointGrabbed()
    {
        currentRocketAmount = Mathf.Max(currentRocketAmount, MinRocketAmountOnGrabAnchorPoint);
        GameCanvas.Instance?.UpdateRocketsUI(currentRocketAmount);
        rocketLauncherAnimator.SetInteger(_ANIMATOR_PARAM_AMMO_COUNT, currentRocketAmount);
    }
}
