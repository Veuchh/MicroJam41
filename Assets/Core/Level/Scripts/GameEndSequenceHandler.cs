using DG.Tweening;
using System;
using System.Numerics;
using UnityEngine;

public class GameEndSequenceHandler : MonoBehaviour
{
    const string PLAYER_TAG = "Player";

    [SerializeField]
    float targetTimeScale = .33f;

    [SerializeField]
    float timeScaleLoweringDuration = .5f;

    [SerializeField]
    float timeScaleLowInterval = 2f;

    [SerializeField]
    float timeScaleResetDuration = .5f;

    [SerializeField]
    Transform plane;

    [SerializeField]
    Transform planeResetPosition;

    Sequence timeScaleSequence;
    bool isGameEnded = false;

    private void Awake()
    {
        AnchorPoint.OnEndAnchorPointGrabbed += EndPointGrabbed;
    }

    private void OnDestroy()
    {
        AnchorPoint.OnEndAnchorPointGrabbed -= EndPointGrabbed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isGameEnded && other.CompareTag(PLAYER_TAG))
        {
            plane.position = planeResetPosition.position;

            TryKillTween();

            timeScaleSequence = DOTween.Sequence();

            timeScaleSequence.Append(DOTween.To(
                () => Time.timeScale,
                x => Time.timeScale = x,
                targetTimeScale,
                timeScaleLoweringDuration));

            timeScaleSequence.AppendInterval(timeScaleLowInterval);

            timeScaleSequence.Append(DOTween.To(
                () => Time.timeScale,
                x => Time.timeScale = x,
                1,
                timeScaleResetDuration));
        }
    }

    private void EndPointGrabbed()
    {
        isGameEnded = true;

        TryKillTween();

        timeScaleSequence = DOTween.Sequence();

        timeScaleSequence.Append(DOTween.To(
            () => Time.timeScale,
            x => Time.timeScale = x,
            1,
            timeScaleResetDuration));
    }

    private void TryKillTween()
    {
        if (timeScaleSequence != null) 
        {
            timeScaleSequence.Kill();
        }
    }
}
