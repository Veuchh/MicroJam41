using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio {
    public class AudioPool : MonoBehaviour {
        private readonly List<AudioSource> _pool = new ();
        [SerializeField] private AudioSource _default;

        private void Awake() {
            _pool.Add(_default);
        }

        public CancellationTokenSource Play2D(AudioResource resource) {
            AudioSource audioSource = Get();
            audioSource.resource = resource;
            audioSource.spatialBlend = 0;
            audioSource.Play();
            return GetCanceler(audioSource);
        }

        public CancellationTokenSource Play3D(AudioResource resource, Vector2 position, out Action<Vector2> updatePosition) {
            AudioSource audioSource = Get();
            audioSource.resource = resource;
            audioSource.transform.position = position;
            audioSource.spatialBlend = 1;
            audioSource.Play();
            
            CancellationTokenSource canceler = GetCanceler(audioSource);
            CancellationToken token = canceler.Token;
            updatePosition = newPos => {
                if (token.IsCancellationRequested) return;
                audioSource.transform.position = newPos;
            };
            return canceler;
        }

        private static CancellationTokenSource GetCanceler(AudioSource audioSource) {
            CancellationTokenSource canceler = new();
            DisposeHandler(canceler).Forget();
            return canceler;

            async UniTaskVoid DisposeHandler(CancellationTokenSource cancellation) {
                bool canceled = await UniTask.WaitUntil(() => !audioSource.isPlaying, cancellationToken: cancellation.Token).SuppressCancellationThrow();
                if (canceled && audioSource.isPlaying) audioSource.Stop();
                cancellation.Cancel();
            }
        }

        private AudioSource Get() {
            AudioSource source = _pool.FirstOrDefault(s => !s.isPlaying);
            if (!source) source = New();
            return source;
        }

        private AudioSource New() {
            AudioSource source = Instantiate(_default, transform);
            _pool.Add(source);
            return source;
        }
    }
}