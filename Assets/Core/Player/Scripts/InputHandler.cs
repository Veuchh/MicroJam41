using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private PlayerRocket playerRocket;

    Camera mainCamera;

    Camera MainCamera => mainCamera ??= Camera.main;

    public void OnUpdateMousePosition(InputValue value)
    {
        Vector2 mouseScreenPos = value.Get<Vector2>();
        Vector3 frogScreenPos = MainCamera.WorldToScreenPoint(transform.position);

        Vector2 direction = mouseScreenPos - new Vector2(frogScreenPos.x, frogScreenPos.y);

        playerRocket.UpdateRocketLauncherDirection(direction);
    }


    public void OnFire(InputValue value)
    {
        playerRocket.OnFireClicked();
    }
}
