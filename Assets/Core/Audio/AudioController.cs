using System;
using System.Threading;
using Core.Player;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio {
    public class AudioController : MonoBehaviour {
        
        
        private readonly CompositeDisposable _rocketEventBindings = new();
        [Header("Rocket")]
        [SerializeField] private AudioPool _rocketFireSource;
        [SerializeField] private AudioResource _rocketFire;
        
        [SerializeField] private AudioPool _rocketTrailSource;
        [SerializeField] private AudioResource _rocketTrail;
        
        [SerializeField] private AudioPool _rocketExplosionSource;
        [SerializeField] private AudioResource _rocketExplosion;

        private void Awake() {
            _rocketEventBindings.AddTo(this);
        }

        private void OnEnable() {
            Bind();
        }

        private void OnDisable() {
            Unbind();
        }

        private void Bind() {
            BindRocketEvents();
        }
        private void Unbind() {
            UnbindRocketEvents();
        }


        #region Rocket Events
        
        private void BindRocketEvents() {
            _rocketEventBindings.Add(RocketMissile.OnSpawned.AsObservable().Subscribe(_ => {
                _rocketFireSource.Play2D(_rocketFire);
            }));
            
            _rocketEventBindings.Add(RocketMissile.OnSpawned.AsObservable().Subscribe(ctx => {
                int instanceID = ctx.Item1;
                RocketMissile missile = RocketMissile.GetByID(instanceID);
                if (!missile) return;
                // Start the trail audio
                CancellationTokenSource audioCanceler = _rocketTrailSource.Play3D(_rocketTrail, position: ctx.Item2, out Action<Vector2> updatePositionHandler);
                // Updates the 3D location of the audiosource as the missile moves
                IDisposable updatePosCanceler = missile.ObserveEveryValueChanged(m => m.transform.position)
                    .Subscribe(pos => updatePositionHandler(pos));
                // Stop updating position when the missile is destroyed (not registered anymore)
                UniTask.WaitUntil(() => !RocketMissile.GetByID(instanceID)).ContinueWith(() => {
                    audioCanceler.Cancel();
                    updatePosCanceler.Dispose();
                });
            }));
            
            _rocketEventBindings.Add(RocketMissile.OnExplosion.AsObservable().Subscribe(ctx => {
                _rocketExplosionSource.Play3D(_rocketExplosion, position: ctx.Item2, out _);
            }));
        }
        
        private void UnbindRocketEvents() {
            _rocketEventBindings?.Dispose();
        }
        
        #endregion
    }
}
