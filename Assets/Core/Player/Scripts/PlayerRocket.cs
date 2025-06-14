using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerRocket : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [Header("Rocket Refill")]
    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] float rocketRefillRaycastDist;
    [SerializeField] int maxRocketAmount;
    [SerializeField] float rocketCooldown = .1f;
    [Header("Rocket Inertia")]
    [SerializeField] float rocketStrength;
    [SerializeField] float baseMomentumMultiplierOnFireIfSameDir = .5f;
    [SerializeField] float momentumMultiplierOnFireSameDirection = .25f;
    [SerializeField] float momentumMultiplierOnFireOppositeDirection = .1f;
    [Header("Rocket GFX")]
    [SerializeField] Transform rocketLauncher;

    int currentRocketAmount;
    float nextAllowedRocketShot;
    Vector2 currentRocketLauncherDirection;

    private void Awake()
    {
        RefillRockets();
    }

    private void Update()
    {
        rocketLauncher.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, currentRocketLauncherDirection) + 90);

        if (rb.linearVelocityY <= 0 && Physics2D.Raycast(transform.position, Vector2.down, rocketRefillRaycastDist, platformLayerMask))
        {
            RefillRockets();
        }
    }

    public void RefillRockets()
    {
        currentRocketAmount = maxRocketAmount;
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
    }

    private void FireRocket()
    {
        nextAllowedRocketShot = Time.time + rocketCooldown;
        currentRocketAmount--;

        float dotProduct = Vector2.Dot(rb.linearVelocity, -currentRocketLauncherDirection);

        if (dotProduct > 0)
            rb.linearVelocity *= (1 / dotProduct) * momentumMultiplierOnFireSameDirection + baseMomentumMultiplierOnFireIfSameDir;
        else
            rb.linearVelocity *= momentumMultiplierOnFireOppositeDirection;

        rb.AddForce(-currentRocketLauncherDirection * rocketStrength, ForceMode2D.Impulse);
    }
}
