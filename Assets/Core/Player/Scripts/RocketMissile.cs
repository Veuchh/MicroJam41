using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Player {
    public class RocketMissile : MonoBehaviour {
        #region Static

        public static readonly UnityEvent<int,Vector2> OnExplosion = new();
        public static readonly UnityEvent<int,Vector2> OnSpawned = new();
        private static readonly Dictionary<int, (RocketMissile instance, float deathTime)> _LIVING_MISSILES = new();
        public static RocketMissile GetByID(int id) {
            CleanRegistry();
            return _LIVING_MISSILES.TryGetValue(id, out (RocketMissile instance, float deathTime) missile)
                ? missile.instance : null;
        }

        private static void CleanRegistry() {
            foreach (int id in _LIVING_MISSILES.Keys.ToList()) {
                if (!_LIVING_MISSILES.TryGetValue(id, out (RocketMissile instance, float deathTime) missile)) continue;
                
                if (!missile.instance || Time.time > missile.deathTime) {
                    _LIVING_MISSILES.Remove(id);
                }
            }
        }

        private static void Register(RocketMissile instance, float lifetime) {
            _LIVING_MISSILES[instance.GetInstanceID()] = (instance, lifetime);
        }

        private static void Unregister(RocketMissile instance) {
            _LIVING_MISSILES.Remove(instance.GetInstanceID());
        }

        #endregion

        
        public float lifeTime = 5f;
        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _acceleration = 0;
        [SerializeField] private float _velocity = 1;
        [SerializeField] private ParticleSystem[] _particleSystems;
        private CancellationTokenSource _lifeTimeKillCts;


        private void OnEnable() {
            Register(this,Time.time + lifeTime);
            OnSpawned.Invoke(GetInstanceID(), transform.position);
            _lifeTimeKillCts = new CancellationTokenSource();
            KillAfter(lifeTime,_lifeTimeKillCts.Token).Forget();
        }

        private void OnDisable() {
            Unregister(this);
            Unbind();
        }

        private void Unbind() {
            _lifeTimeKillCts?.Cancel();
            _lifeTimeKillCts = null;
        }

        private void FixedUpdate() {
            UpdateVelocity();
            UpdateCollision();
        }

        private void Update() {
            transform.position += Time.deltaTime * _velocity * -transform.right;
        }

        private void UpdateVelocity() {
            _velocity += _acceleration * Time.deltaTime;
        }

        private void UpdateCollision() {
            List<Collider2D> colliders = new();
            if (Physics2D.OverlapCollider(_collider, new ContactFilter2D { layerMask = _layerMask, useLayerMask = true, }, colliders) > 0) {             
                Explode();
            }
        }

        private async UniTaskVoid KillAfter(float duration, CancellationToken token) {
            bool canceled = await UniTask.WaitForSeconds(duration, cancellationToken: token).SuppressCancellationThrow();
            if (canceled) return;
            Explode();
        }

        private void Explode() {
            Unbind();

            if (_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            OnExplosion.Invoke(GetInstanceID(), transform.position);
            DelayParticleSystemsDestruction();
            Destroy(gameObject);
        }

        private void DelayParticleSystemsDestruction() {
            foreach (ParticleSystem ps in _particleSystems.Where(ps => ps)) {
                ps.transform.parent = null;
                ParticleSystem.MainModule main = ps.main;
                main.loop = false;
            }
        }
    }
}