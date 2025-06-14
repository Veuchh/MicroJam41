using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerRocket : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float rocketStrength;
    [SerializeField] float baseMomentumMultiplierOnFireIfSameDir = .5f;
    [SerializeField] float momentumMultiplierOnFireSameDirection = .25f;
    [SerializeField] float momentumMultiplierOnFireOppositeDirection = .1f;
    [SerializeField] Transform rocketLauncher;

    Vector2 currentRocketLauncherDirection;

    private void Update()
    {
        rocketLauncher.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, currentRocketLauncherDirection) + 90);
    }

    public void UpdateRocketLauncherDirection(Vector2 newDirection)
    {
        currentRocketLauncherDirection = newDirection.normalized;
    }

    public void OnFireClicked()
    {
        //TODO : check remaining rockets & cooldown
        if (false)
            return;

        FireRocket();
    }

    private void FireRocket()
    {
        if (rb != null)
        {

        }

        float dotProduct = Vector2.Dot(rb.linearVelocity, -currentRocketLauncherDirection);

        if (dotProduct > 0)
            rb.linearVelocity *= (1 / dotProduct) * momentumMultiplierOnFireSameDirection + baseMomentumMultiplierOnFireIfSameDir;
        else
            rb.linearVelocity *= momentumMultiplierOnFireOppositeDirection;

        rb.AddForce(-currentRocketLauncherDirection * rocketStrength, ForceMode2D.Impulse);
    }
}
