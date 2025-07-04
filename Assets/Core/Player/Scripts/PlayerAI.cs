using Core.Player.Data;
using Unity.Cinemachine;
using UnityEngine;

namespace Core.Player {
    public class PlayerAI : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private float aimPerlinSpeed = 1f;

        [SerializeField, MinMaxRangeSlider(0f,2f)]
        Vector2 minMaxSleepTime;

        PlayerRocket playerRocket;
        float randomYForXRotation;
        float randomYForYRotation;
        float nextRocketFire;

        private void Awake()
        {
            playerRocket = GetComponent<PlayerRocket>();
            randomYForXRotation = Random.Range(0f, 100f);
            randomYForYRotation = Random.Range(0f, 100f);
        }

        private void Update()
        {
            float newXAimDir = Mathf.PerlinNoise(Time.time * aimPerlinSpeed, randomYForXRotation) - .5f;
            float newYAimDir = Mathf.PerlinNoise(Time.time * aimPerlinSpeed, randomYForYRotation) -.5f;

            _playerData.currentAimDirection.Value = new Vector2(newXAimDir, newYAimDir).normalized;

            if (Time.time <= nextRocketFire) return;
            nextRocketFire = Time.time + Random.Range(minMaxSleepTime.x, minMaxSleepTime.y);
            playerRocket.OnFireClicked();
        }
    }
}
