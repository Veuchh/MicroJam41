using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

public class RocketRefillCollectible : MonoBehaviour {
    public static event Action<Vector2, int> OnAmmoCollected;
    private static readonly int _COLLECTED = Animator.StringToHash("Collected");
    [SerializeField, FormerlySerializedAs("rocketsToAdd")] private int _ammoCount = 1;

    [SerializeField, Tag] private string _playerTag;
    [SerializeField, FormerlySerializedAs("cooldownDuration")] private float _cooldownDuration = 2f;
    [SerializeField] private Animator _animator;


    private bool _isCollected;

    private IDisposable _binding;
    private CancellationTokenSource _respawnCts;

    private bool CanBePicked => !_isCollected;

    public void ResetCollectible() {
        Unbind();
        SetCollected(false);
    }

    private void SetCollected(bool collected) {
        _isCollected = collected;
        _animator.SetBool(_COLLECTED, _isCollected);
    }

    private void OnEnable() {
        _binding = this.OnTriggerStay2DAsObservable()
            .Where(other => other.CompareTag(_playerTag) && CanBePicked)
            .Subscribe(_ => Collect());
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
        RespawnIn(_cooldownDuration);
        OnAmmoCollected?.Invoke(transform.position, _ammoCount);
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
            SetCollected(false);
        }
    }
}