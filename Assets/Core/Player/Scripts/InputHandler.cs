using Core.Player;
using Core.Player.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private PlayerRocket playerRocket;
    [SerializeField] private PlayerData _playerData;
    
    [SerializeField] 
    AnchorPointHandler anchorPointHandler;

    Camera mainCamera;

    Camera MainCamera => mainCamera ??= Camera.main;

    public void OnUpdateMousePosition(InputValue value)
    {
        Vector2 mouseScreenPos = value.Get<Vector2>();
        Vector3 frogScreenPos = MainCamera.WorldToScreenPoint(transform.position);

        Vector2 direction = mouseScreenPos - new Vector2(frogScreenPos.x, frogScreenPos.y);
        _playerData.currentAimDirection.Value = direction.normalized;
    }


    public void OnFire(InputValue value)
    {
        playerRocket.OnFireClicked();
    }

    public void OnHoldAnchorPoint(InputValue value)
    {
        anchorPointHandler.OnNewAnchorPointInput(value.Get<float>() > .5f);
    }

    public void OnUpdateJoystickDirection(InputValue value)
    {
        _playerData.currentAimDirection.Value = value.Get<Vector2>().normalized;
    }
}
