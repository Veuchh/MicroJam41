using NaughtyAttributes;
using UnityEngine;

namespace Core.Player {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AnchorPointHandler))]
    public class PlayerRocket : MonoBehaviour {
        public delegate void RocketFireEvent(Vector2 pos, Vector2 dir);
        public static event RocketFireEvent OnRocketFired;
    
        private Rigidbody2D Rigidbody => _rigidbody ??= GetComponent<Rigidbody2D>();
        private Rigidbody2D _rigidbody;
        private AnchorPointHandler AnchorPointHandler => _anchorPointHandler ??= GetComponent<AnchorPointHandler>();
        private AnchorPointHandler _anchorPointHandler;
        
        [Foldout("Rocket Refill")] [SerializeField] LayerMask platformLayerMask;
        [Foldout("Rocket Refill")] [SerializeField] float rocketRefillRaycastDist;
        [Foldout("Rocket Refill")] [SerializeField] int maxRocketAmount;
        [Foldout("Rocket Refill")] [SerializeField] int MinRocketAmountOnGrabAnchorPoint = 1;
        [Foldout("Rocket Refill")] [SerializeField] float rocketCooldown = .1f;
        [Space]
        [Foldout("Rocket Inertia")] [SerializeField] float slingshotStrengthMultiplier = 1;
        [Foldout("Rocket Inertia")] [SerializeField] float rocketStrength;
        [Foldout("Rocket Inertia")] [SerializeField] float baseMomentumMultiplierOnFireIfSameDir = .5f;
        [Foldout("Rocket Inertia")] [SerializeField] float momentumMultiplierOnFireSameDirection = .25f;
        [Foldout("Rocket Inertia")] [SerializeField] float momentumMultiplierOnFireOppositeDirection = .1f;

        [Space] [Foldout("GFX")] [SerializeField] private RocketLauncherVisualHandler _rocketVisuals;
        [Foldout("GFX")] [SerializeField] Transform eyeLeft;
        [Foldout("GFX")] [SerializeField] Transform eyeRight;

        private int currentRocketAmount;
        private float nextAllowedRocketShot;
        private Vector2 currentRocketLauncherDirection;
        
        private void Start()
        {
            RefillRockets();
        }

        private void Update() {
            eyeLeft.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, currentRocketLauncherDirection));
            eyeRight.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, currentRocketLauncherDirection));

            if (Rigidbody.linearVelocityY <= 0 && Physics2D.Raycast(transform.position, Vector2.down, rocketRefillRaycastDist, platformLayerMask))
            {
                RefillRockets();
            }
        }

        public void RefillRockets()
        {
            currentRocketAmount = maxRocketAmount;
            _rocketVisuals.SetAmmoCount(currentRocketAmount);
            GameCanvas.Instance?.UpdateRocketsUI(currentRocketAmount);
        }

        public void UpdateRocketLauncherDirection(Vector2 newDirection) {
            _rocketVisuals.SetDirection(newDirection.normalized);
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
            _rocketVisuals.Fire(currentRocketAmount);

            GameCanvas.Instance?.UpdateRocketsUI(currentRocketAmount);

            float dotProduct = Vector2.Dot(Rigidbody.linearVelocity, -currentRocketLauncherDirection);

            if (dotProduct > 0)
                Rigidbody.linearVelocity *= (1 / dotProduct) * momentumMultiplierOnFireSameDirection + baseMomentumMultiplierOnFireIfSameDir;
            else
                Rigidbody.linearVelocity *= momentumMultiplierOnFireOppositeDirection;

            //Slingshotting when shooting perpendicularly to held anchor
            float slingShotMultiplier = 1;
            if (AnchorPointHandler.IsHoldingAnchorPoint)
            {
                Vector2 transformPositionVector2 = new Vector2(transform.position.x, transform.position.y);
                Vector2 anchorPositionVector2 = new Vector2(AnchorPointHandler.HeldAnchorPoint.transform.position.x, AnchorPointHandler.HeldAnchorPoint.transform.position.y);
                Vector2 direction = transformPositionVector2 - anchorPositionVector2;
                float anchorToAimDotProduct = Vector2.Dot(-currentRocketLauncherDirection.normalized, direction.normalized);

                slingShotMultiplier += (- (Mathf.Abs(anchorToAimDotProduct) - 1)) * slingshotStrengthMultiplier;
            }

            Rigidbody.AddForce(-currentRocketLauncherDirection * rocketStrength * slingShotMultiplier, ForceMode2D.Impulse);
            OnRocketFired?.Invoke(transform.position, currentRocketLauncherDirection);
        }

        public void OnAnchorPointGrabbed() {
            currentRocketAmount = Mathf.Max(currentRocketAmount, MinRocketAmountOnGrabAnchorPoint);
            GameCanvas.Instance?.UpdateRocketsUI(currentRocketAmount);
            _rocketVisuals.SetAmmoCount(currentRocketAmount);
        }
    }
}
