using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Audio {
    public static class AudioUtils {

        public static CancellationTokenSource GetCanceler(this AudioSource audioSource) {
            CancellationTokenSource canceler = new();
            DisposeHandler(canceler).Forget();
            return canceler;

            async UniTaskVoid DisposeHandler(CancellationTokenSource cancellation) {
                bool canceled = await UniTask.WaitUntil(() => !audioSource || !audioSource.isPlaying, cancellationToken: cancellation.Token).SuppressCancellationThrow();
                if (canceled && audioSource && audioSource.isPlaying) audioSource.Stop();
                cancellation.Cancel();
            }
        }
    }
}