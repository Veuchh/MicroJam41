using Core.Player.Events;
using UnityEngine;

namespace Core.Player {
    [RequireComponent(typeof(ScreenShaker))]
    public class ScreenShakeTrigger : MonoBehaviour {
        [SerializeField] private PlayerEvents _playerEvents;
        [SerializeField] private float _rocketFiredScreenShakeDuration = 0.5f;
        private ScreenShaker ScreenShaker => _screenShaker ??= GetComponent<ScreenShaker>();
        private ScreenShaker _screenShaker;

        private void OnEnable() {
            if (!_playerEvents) return;
            _playerEvents.onRocketFired.RemoveListener(RocketFiredScreenShake);
            _playerEvents.onRocketFired.AddListener(RocketFiredScreenShake);
        }
        
        private void OnDisable() {
            if (!_playerEvents) return;
            _playerEvents.onRocketFired.RemoveListener(RocketFiredScreenShake);
        }
        
        private void RocketFiredScreenShake(PlayerEvents.RocketFiredData data) {
            if (ScreenShaker) ScreenShaker.ScreenShakeForDuration(_rocketFiredScreenShakeDuration);
        }
    }
}