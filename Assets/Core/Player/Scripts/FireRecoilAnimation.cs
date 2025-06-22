using Core.Player.Data;
using Core.Player.Events;
using DG.Tweening;
using UnityEngine;

namespace Core.Player {
    public class FireRecoilAnimation : MonoBehaviour {
        [SerializeField] private PlayerSettings _playerSettings;
        [SerializeField] private PlayerEvents _playerEvents;
        [SerializeField] private Transform _recoilTransform;
        
        private Sequence _recoilSequence;
        private Vector3 _recoilBasePosition;
        
        private void Awake() {
            _recoilBasePosition = _recoilTransform.localPosition;
        }

        private void OnEnable() {
            _playerEvents.onRocketFireRequested.RemoveListener(OnRocketFired);
            _playerEvents.onRocketFireRequested.AddListener(OnRocketFired);
        }

        private void OnDisable() {
            _playerEvents.onRocketFireRequested.RemoveListener(OnRocketFired);
        }

        private void OnRocketFired(PlayerEvents.RocketFiredData data) {
            Animate();
        }

        private void Animate()
        {
            _recoilSequence?.Kill();
            _recoilSequence = DOTween.Sequence();
            _recoilSequence.OnKill(() => _recoilTransform.localPosition = _recoilBasePosition);

            float targetRecoil = Mathf.Min(_recoilTransform.localPosition.z + _playerSettings.RecoilStrength, _playerSettings.RecoilMaxDistance);
            _recoilSequence.Join(_recoilTransform.DOLocalMoveX(targetRecoil, _playerSettings.RecoilDuration).SetEase(Ease.OutCirc));
            _recoilSequence.Append(_recoilTransform.DOLocalMoveX(0, _playerSettings.RecoilRecoverDuration).SetEase(Ease.InOutSine));
        }
    }
}