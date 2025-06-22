using Core.Player.Events;
using UnityEngine;

namespace Core.Player {
    public class RocketSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerEvents _playerEvents;
        [SerializeField] private RocketMissile _rocketPrefab;

        private void OnEnable() {
            if (!_playerEvents) return;
            _playerEvents.onRocketFireRequested.RemoveListener(OnRocketFired);
            _playerEvents.onRocketFireRequested.AddListener(OnRocketFired);
        }

        private void OnDisable()
        {
            if (!_playerEvents) return;
            _playerEvents.onRocketFireRequested.RemoveListener(OnRocketFired);
        }

        private void OnRocketFired(PlayerEvents.RocketFiredData data) {
            RocketMissile rocket = Instantiate(_rocketPrefab, transform.position, Quaternion.identity);
            rocket.transform.rotation = Quaternion.AngleAxis(-Vector2.SignedAngle(data.direction.normalized, Vector3.left),Vector3.forward);

            if (_playerEvents) {
                _playerEvents.onRocketFired.Invoke(new PlayerEvents.RocketFiredData {
                    position = transform.position,
                    direction = data.direction,
                });
            }
        }
    }
}