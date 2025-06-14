using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class RocketUI : MonoBehaviour
{
    [SerializeField] float rotationStrength = 90;
    [SerializeField] float shakeStrength = 90;
    [SerializeField] int shakeVibrato = 90;
    [SerializeField] float fadeInTweenDuration = .2f;
    [SerializeField] float fadeOutTweenDuration = .2f;

    CanvasGroup canvasGroup;
    Sequence currentSequence;
    bool isShown = false;

    public bool IsShown => isShown;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void KillTween()
    {
        if (currentSequence != null)
            currentSequence.Kill();

        currentSequence = null;
    }

    [Button]
    public void OnRocketFired()
    {
        KillTween();

        isShown = false;
        
        transform.rotation = Quaternion.identity;
        canvasGroup.alpha = 1;

        currentSequence = DOTween.Sequence();
        currentSequence.Join(transform.DOShakePosition(fadeOutTweenDuration, shakeStrength, shakeVibrato));
        currentSequence.Join(canvasGroup.DOFade(0, fadeOutTweenDuration));

    }

    [Button]
    public void OnRocketRecovered(float durationDelay = 0)
    {
        isShown = true;
        KillTween();
        currentSequence = DOTween.Sequence();
        currentSequence.AppendInterval(durationDelay);
        currentSequence.AppendCallback(() => transform.rotation = Quaternion.Euler(0,0,rotationStrength));
        currentSequence.AppendCallback(() => canvasGroup.alpha = 0);
        currentSequence.Join(transform.DORotate(Vector3.zero, fadeInTweenDuration).SetEase(Ease.OutQuad));
        currentSequence.Join(canvasGroup.DOFade(1,fadeInTweenDuration));
    }
}
