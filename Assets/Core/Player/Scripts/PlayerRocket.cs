using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerRocket : MonoBehaviour
{
    [SerializeField] Transform rocketLauncher;

    Vector2 currentRocketLauncherDirection;

    public void UpdateRocketLauncherDirection(Vector2 newDirection)
    {
        currentRocketLauncherDirection = newDirection.normalized;
    }

    private void Update()
    {
        rocketLauncher.rotation = Quaternion.Euler(0,0, Vector2.SignedAngle(Vector2.up, currentRocketLauncherDirection) + 90);
    }
}
