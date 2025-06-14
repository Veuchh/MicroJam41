using UnityEngine;

namespace Core.Player {
    public class RocketLauncherVisualHandler : MonoBehaviour {
        private static readonly int _ANIMATOR_PARAM_AMMO_COUNT = Animator.StringToHash("AmmoCount");
        private static readonly int _ANIMATOR_PARAM_FIRE_TRIGGER = Animator.StringToHash("Fire");
        [SerializeField] private Animator _rocketLauncherAnimator;
        [SerializeField] private SpriteRenderer _rocketLauncherSprite;
        private Vector2 _rocketLauncherDirection;

        public void SetAmmoCount(int currentRocketAmount) {
            _rocketLauncherAnimator.SetInteger(_ANIMATOR_PARAM_AMMO_COUNT, currentRocketAmount);
        }

        public void Fire(int currentRocketAmount) {
            SetAmmoCount(currentRocketAmount);
            _rocketLauncherAnimator.SetTrigger(_ANIMATOR_PARAM_FIRE_TRIGGER);
        }

        public void SetDirection(Vector2 direction) {
            _rocketLauncherDirection = direction;
        }

        private void Update() {
            float rocketLauncherRotation = Vector2.SignedAngle(Vector2.up, _rocketLauncherDirection) - 90;
            transform.rotation = Quaternion.Euler(0, 0, rocketLauncherRotation);
            _rocketLauncherSprite.flipY = rocketLauncherRotation is < -90 or > 90;
        }
    }
}