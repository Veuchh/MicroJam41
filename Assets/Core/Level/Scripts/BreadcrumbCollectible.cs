using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Level {
    [RequireComponent(typeof(Collider2D))]
    public class BreadcrumbCollectible : MonoBehaviour {
        public static event Action<Vector2> OnBreadcrumbCollected;
        private static readonly int _COLLECTED = Animator.StringToHash("Collected");
        private static readonly int _VISIBLE = Animator.StringToHash("Visible");
        private static readonly int _HIDE_INSTANT = Animator.StringToHash("HideInstant");
        private static readonly int _IDLE_OFFSET = Animator.StringToHash("IdleOffset");
        
        [SerializeField, Tag] private string _playerTag;
        [SerializeField] private float _cooldownDuration = 2f;

        [SerializeField] private Animator _animator;

        private float _cooldownEnd;
        private bool _isVisible = true;
        private bool _isCollected;

        private IDisposable _binding;
        private CancellationTokenSource _respawnCts;

        private bool CanBePicked => _isVisible && ! _isCollected;

        public void ResetCollectible() {
            Unbind();
            SetCollected(false);
            SetVisible(true);
        }

        private void SetCollected(bool collected) {
            _isCollected = collected;
            _animator.SetBool(_COLLECTED, _isCollected);
        }
        
        private void SetVisible(bool visible) {
            _isVisible = visible;
            _animator.SetBool(_VISIBLE, _isVisible);
        }

        private void Awake() {
            _animator.SetFloat(_IDLE_OFFSET, Random.value);
        }

        private void OnEnable() {
            _binding = this.OnTriggerStay2DAsObservable()
                .Where(other => other.CompareTag(_playerTag) && CanBePicked)
                .Subscribe(_ => Collect());
            
            SetCollected(_isCollected);
            SetVisible(_isVisible);
            RespawnIn(0f);
            _animator.SetTrigger(_HIDE_INSTANT);
        }

        private void OnDisable() {
            Unbind();
        }

        private void Unbind() {
            _binding?.Dispose();
            _binding = null;
            _respawnCts?.Cancel();
            _respawnCts = null;
        }

        private void Collect() {
            SetCollected(true);
            SetVisible(false);
            RespawnIn(_cooldownDuration);
            OnBreadcrumbCollected?.Invoke(transform.position);
        }

        private void RespawnIn(float cooldownDuration) {
            if (cooldownDuration == 0) Respawn();
            else {
                _respawnCts = new CancellationTokenSource();
                RespawnRoutine(cooldownDuration, _respawnCts.Token).Forget();
            }
            return;

            async UniTaskVoid RespawnRoutine(float delay, CancellationToken cancellationToken) {
                bool canceled = await UniTask.WaitForSeconds(delay, cancellationToken: cancellationToken).SuppressCancellationThrow();
                if (canceled) return;
                Respawn();
            }

            void Respawn() {
                SetVisible(true);
            }
        }
    }
}