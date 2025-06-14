using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private PlayerRocket playerRocket;

    public void OnUpdateMousePosition(InputValue value)
    {
        Vector2 newRocketLauncherDirection = value.Get<Vector2>() / new Vector2(Screen.width, Screen.height);

        newRocketLauncherDirection -= Vector2.one / 2;
        playerRocket.UpdateRocketLauncherDirection(newRocketLauncherDirection);
    }
}
