using Core.Player.Data;
using Core.Player.Events;
using NaughtyAttributes;
using UniRx;
using UnityEngine;

namespace Core.Player {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AnchorPointHandler))]
    public class PlayerRocket : MonoBehaviour {
        private Rigidbody2D Rigidbody => _rigidbody ??= GetComponent<Rigidbody2D>();
        private Rigidbody2D _rigidbody;
        private AnchorPointHandler AnchorPointHandler => _anchorPointHandler ??= GetComponent<AnchorPointHandler>();
        private AnchorPointHandler _anchorPointHandler;
        
        [SerializeField] private PlayerEvents _playerEvents;
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private PlayerSettings _playerSettings;

        [Foldout("GFX")] [SerializeField] private RocketLauncherVisualHandler _rocketVisuals;
        [Foldout("GFX")] [SerializeField] Transform eyeLeft;
        [Foldout("GFX")] [SerializeField] Transform eyeRight;

        private int currentRocketAmount;
        private float nextAllowedRocketShot;
        private readonly CompositeDisposable _bindings = new();


        private void OnDestroy() {
            _bindings.Dispose();
        }

        private void OnEnable() {
            RocketRefillCollectible.OnAmmoCollected -= OnCollectiblePicked;
            RocketRefillCollectible.OnAmmoCollected += OnCollectiblePicked;
            _bindings.Add(_playerData.currentAimDirection.Subscribe(OnAimDirectionChanged));
        }

        private void OnDisable() {
            _bindings.Clear();
            RocketRefillCollectible.OnAmmoCollected -= OnCollectiblePicked;
        }

        private void Start() {
            RefillRockets();
        }

        private void Update() {
            eyeLeft.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, _playerData.currentAimDirection.Value));
            eyeRight.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, _playerData.currentAimDirection.Value));

            if (Rigidbody.linearVelocityY <= 0 && IsGrounded()) {
                RefillRockets();
            }
        }

        private bool IsGrounded() => _rigidbody.IsTouching(_playerSettings.GroundCheckContactFilter);
        // {
        //     return 
        //         Physics2D.Raycast(transform.position, Vector2.down, _playerStats.GroundCheckDistance, _playerStats.PlatformLayerMask)
        //         ||Physics2D.Raycast(transform.position + Vector3.left * _playerStats.SideRaycastDistance, Vector2.down, _playerStats.GroundCheckDistance, _playerStats.PlatformLayerMask)
        //         ||Physics2D.Raycast(transform.position + Vector3.right * _playerStats.SideRaycastDistance, Vector2.down, _playerStats.GroundCheckDistance, _playerStats.PlatformLayerMask);
        // }

        public void RefillRockets()
        {
            currentRocketAmount = _playerSettings.MaxRocketAmount;
            _rocketVisuals.SetAmmoCount(currentRocketAmount);
            if (GameCanvas.Instance) GameCanvas.Instance.UpdateRocketsUI(currentRocketAmount);
        }
        
        private void OnCollectiblePicked(Vector2 position, int ammoCount) => AddRockets(ammoCount);
        
        public void AddRockets(int rocketsToAdd) {
            currentRocketAmount = Mathf.Min(currentRocketAmount + rocketsToAdd, _playerSettings.MaxRocketAmount);
            _rocketVisuals.SetAmmoCount(currentRocketAmount);
            if(GameCanvas.Instance) GameCanvas.Instance.UpdateRocketsUI(currentRocketAmount);
        }

        private void OnAimDirectionChanged(Vector2 direction) {
            _rocketVisuals.SetDirection(direction);
        }

        public void OnFireClicked()
        {
            if (currentRocketAmount <= 0 || Time.time < nextAllowedRocketShot)
                return;
            FireRocket();
        }

        private void FireRocket()
        {
            nextAllowedRocketShot = Time.time + _playerSettings.RocketCooldown;
            currentRocketAmount--;
            _rocketVisuals.Fire(currentRocketAmount);

            GameCanvas.Instance?.UpdateRocketsUI(currentRocketAmount);

            Vector2 aimDirection = _playerData.currentAimDirection.Value;
            float dotProduct = Vector2.Dot(Rigidbody.linearVelocity, -aimDirection);

            if (dotProduct > 0)
                Rigidbody.linearVelocity *= (1 / dotProduct) * _playerSettings.MomentumMultiplierOnFireSameDirection + _playerSettings.BaseMomentumMultiplierOnFireIfSameDir;
            else
                Rigidbody.linearVelocity *= _playerSettings.MomentumMultiplierOnFireOppositeDirection;

            //Slingshotting when shooting perpendicularly to held anchor
            float slingShotMultiplier = 1;
            if (AnchorPointHandler.IsHoldingAnchorPoint)
            {
                Vector2 transformPositionVector2 = new (transform.position.x, transform.position.y);
                Vector2 anchorPositionVector2 = new (AnchorPointHandler.HeldAnchorPoint.transform.position.x, AnchorPointHandler.HeldAnchorPoint.transform.position.y);
                Vector2 direction = transformPositionVector2 - anchorPositionVector2;
                float anchorToAimDotProduct = Vector2.Dot(-aimDirection, direction.normalized);

                slingShotMultiplier += (- (Mathf.Abs(anchorToAimDotProduct) - 1)) * _playerSettings.SlingshotStrengthMultiplier;
            }

            Rigidbody.AddForce(-aimDirection * _playerSettings.RocketStrength * slingShotMultiplier, ForceMode2D.Impulse);
            
            if (_playerEvents) {
                _playerEvents.onRocketFireRequested.Invoke(new PlayerEvents.RocketFiredData {
                    position = transform.position, 
                    direction = aimDirection
                });
            }
        }

        public void OnAnchorPointGrabbed() {
            currentRocketAmount = Mathf.Max(currentRocketAmount, _playerSettings.MinRocketAmountOnGrabAnchorPoint);
            GameCanvas.Instance?.UpdateRocketsUI(currentRocketAmount);
            _rocketVisuals.SetAmmoCount(currentRocketAmount);
        }
    }
}
