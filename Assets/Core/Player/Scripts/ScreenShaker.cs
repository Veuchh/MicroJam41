using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace Core.Player {
    public class ScreenShaker : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private float screenShakeAmplitude;
        [SerializeField] private float screenShakeFrequency;


        private CinemachineBasicMultiChannelPerlin _cinemachineNoise;
        private bool _isScreenShakeOn;
        private float _screenShakeEndTime;
        private CancellationTokenSource _screenShakeCts;

        private void Awake()
        {
            cinemachineCamera.transform.parent = null;
            _cinemachineNoise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            StopScreenShake();
        }

        private void OnDisable() {
            _screenShakeCts?.Cancel();
        }

        public void ScreenShakeForDuration(float duration) {
            if (Time.time + duration <= _screenShakeEndTime) return;
            _screenShakeCts?.Cancel();
            _screenShakeCts = new CancellationTokenSource();
            ScreenShakeTask(duration, _screenShakeCts.Token).Forget();
        }

        private async UniTask ScreenShakeTask(float duration, CancellationToken cancellation = default) {
            StartScreenShake(duration);
            bool canceled = await UniTask.WaitForSeconds(duration, cancellationToken: cancellation).SuppressCancellationThrow();
            if (!canceled) StopScreenShake();
        }

        private void StartScreenShake(float duration) {
            _isScreenShakeOn = true;
            _screenShakeEndTime = Time.time + duration;
            if (!_cinemachineNoise) return;

            _cinemachineNoise.AmplitudeGain = screenShakeAmplitude;
            _cinemachineNoise.FrequencyGain = screenShakeFrequency;
            _cinemachineNoise.enabled = true;
        }

        private void StopScreenShake() {
            _isScreenShakeOn = false;
            if (!_cinemachineNoise) return;
        
            _cinemachineNoise.AmplitudeGain = 0;
            _cinemachineNoise.FrequencyGain = 0;
            _cinemachineNoise.enabled = false;
        }
    }
}
