using UnityEngine;

namespace Core.Player {
    public class RocketSpawner : MonoBehaviour
    {
        [SerializeField] private RocketMissile _rocketPrefab;

        private void OnEnable() {
            PlayerRocket.OnRocketFired -= OnRocketFired;
            PlayerRocket.OnRocketFired += OnRocketFired;
        }

        private void OnRocketFired(Vector2 pos, Vector2 dir) {
            RocketMissile rocket = Instantiate(_rocketPrefab, pos, Quaternion.identity);
            rocket.transform.rotation = Quaternion.AngleAxis(-Vector2.SignedAngle(dir.normalized, Vector3.left),Vector3.forward);
        }
    }
}