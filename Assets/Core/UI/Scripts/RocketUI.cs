using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class RocketUI : MonoBehaviour {
    public static event Action OnRocketReloaded;
    [SerializeField] float rotationStrength = 90;
    [SerializeField] float shakeStrength = 90;
    [SerializeField] int shakeVibrato = 90;
    [SerializeField] float fadeInTweenDuration = .2f;
    [SerializeField] float fadeOutTweenDuration = .2f;
    [SerializeField] Vector3 startPosition;

    CanvasGroup canvasGroup;
    Sequence currentSequence;

    private CanvasGroup CanvasGroup => canvasGroup ??= GetComponent<CanvasGroup>(); 
    public bool IsShown { get; private set; } = false;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void KillTween()
    {
        currentSequence?.Kill();
        currentSequence = null;
    }

    [Button]
    public void OnRocketFired()
    {
        KillTween();

        IsShown = false;
        
        transform.rotation = Quaternion.identity;
        CanvasGroup.alpha = 1;

        currentSequence = DOTween.Sequence();
        currentSequence.Join(transform.DOShakePosition(fadeOutTweenDuration, shakeStrength, shakeVibrato));
        currentSequence.Join(CanvasGroup.DOFade(0, fadeOutTweenDuration));

    }

    [Button]
    public void OnRocketRecovered(float durationDelay = 0) {
        IsShown = true;
        KillTween();
        currentSequence = DOTween.Sequence();
        currentSequence.AppendInterval(durationDelay);
        currentSequence.AppendCallback(() => {
            if (transform) {
                transform.localPosition = startPosition;
                transform.rotation = Quaternion.Euler(0,0,rotationStrength);
            }
            if (CanvasGroup) CanvasGroup.alpha = 0;
            OnRocketReloaded?.Invoke();
        });
        currentSequence.Join(transform.DORotate(Vector3.zero, fadeInTweenDuration).SetEase(Ease.OutQuad));
        currentSequence.Join(CanvasGroup.DOFade(1,fadeInTweenDuration));
    }
}
