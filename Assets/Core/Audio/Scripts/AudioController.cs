using System;
using System.Threading;
using Core.Player;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio {
    public class AudioController : MonoBehaviour {
        
        private readonly CompositeDisposable _playerEventBindings = new();
        private readonly CompositeDisposable _rocketEventBindings = new();
        
        [Header("Player")]
        [SerializeField] private AudioSource _grappleOnSource;
        [SerializeField] private AudioResource _grappleOn;
        
        [SerializeField] private AudioSource _grappleOffSource;
        [SerializeField] private AudioResource _grappleOff;
        
        [Header("Rocket")]
        [SerializeField] private AudioPool _rocketFireSource;
        [SerializeField] private AudioResource _rocketFire;
        
        [SerializeField] private AudioPool _rocketTrailSource;
        [SerializeField] private AudioResource _rocketTrail;
        
        [SerializeField] private AudioPool _rocketExplosionSource;
        [SerializeField] private AudioResource _rocketExplosion;

        private void Awake() {
            _playerEventBindings.AddTo(this);
            _rocketEventBindings.AddTo(this);
        }

        private void OnEnable() {
            Bind();
        }

        private void OnDisable() {
            Unbind();
        }

        private void Bind() {
            BindPlayerEvents();
            BindRocketEvents();
        }
        private void Unbind() {
            UnbindPlayerEvents();
            UnbindRocketEvents();
        }

        #region Player Events

        private void BindPlayerEvents() {
            Func<Action<AnchorPoint>, AnchorPointHandler.AnchorPointEvent> anchorPointEventConverter = handler => arg => handler(arg);
            IObservable<AnchorPoint> grappleOn = Observable.FromEvent(
                anchorPointEventConverter,
                handler => AnchorPointHandler.OnAnchorPointGrabbed += handler,
                handler => AnchorPointHandler.OnAnchorPointGrabbed -= handler
            );
            IObservable<AnchorPoint> grappleOff = Observable.FromEvent(
                anchorPointEventConverter,
                handler => AnchorPointHandler.OnAnchorPointReleased += handler,
                handler => AnchorPointHandler.OnAnchorPointReleased -= handler
            );

            CancellationTokenSource grappleOnCanceller = null;
            CancellationTokenSource grappleOffCanceller = null;
            
            _playerEventBindings.Add(grappleOn.Subscribe(_ => {
                grappleOnCanceller?.Cancel();
                grappleOffCanceller?.Cancel();
                grappleOnCanceller = null;
                grappleOffCanceller = null;
                
                _grappleOnSource.resource = _grappleOn;
                _grappleOnSource.spatialBlend = 0;
                _grappleOnSource.Play();
                grappleOnCanceller = _grappleOnSource.GetCanceler();
            }));
            _playerEventBindings.Add(grappleOff.Subscribe(_ => {
                grappleOnCanceller?.Cancel();
                grappleOffCanceller?.Cancel();
                grappleOnCanceller = null;
                grappleOffCanceller = null; 
                _grappleOffSource.resource = _grappleOff;
                _grappleOffSource.spatialBlend = 0;
                _grappleOffSource.Play();
                grappleOffCanceller = _grappleOffSource.GetCanceler();
            }));
        }

        private void UnbindPlayerEvents() {
            _playerEventBindings?.Clear();
        }

        #endregion

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
            _rocketEventBindings?.Clear();
        }
        
        #endregion
    }
}
