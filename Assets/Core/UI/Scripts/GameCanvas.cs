using Core.Level;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public static GameCanvas Instance;

    [SerializeField]
    List<RocketUI> rocketsUI;

    [SerializeField]
    float recoverDelay = .1f;

    [SerializeField]
    Transform anchorPointUI;

    [SerializeField]
    float anchorPointTransitionDuration = .2f;

    [SerializeField]
    TextMeshProUGUI scoreText;

    Sequence anchorPointSequence;
    int currentScore = 0;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            BreadcrumbCollectible.OnBreadcrumbCollected += OnBreadcrumbCollected;
            return;
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        BreadcrumbCollectible.OnBreadcrumbCollected -= OnBreadcrumbCollected;
    }

    private void OnBreadcrumbCollected(Vector2 vector)
    {
        currentScore++;
        scoreText.text = currentScore.ToString("D3");
    }

    public void UpdateRocketsUI(int newRocketsAmount)
    {
        float currentRecoverDelay = 0;

        for (int i = 0; i < rocketsUI.Count; i++)
        {
            RocketUI rocketUI = rocketsUI[i];

            if (!rocketUI.IsShown && i < newRocketsAmount)
            {
                rocketUI.OnRocketRecovered(currentRecoverDelay);
                currentRecoverDelay += recoverDelay;
            }
            else if (rocketUI.IsShown && i >= newRocketsAmount)
            {
                rocketUI.OnRocketFired();
            }
        }
    }

    public void UpdateAnchorPoint(bool isShown)
    {
        if (anchorPointSequence != null)
            anchorPointSequence.Kill();

        anchorPointSequence = DOTween.Sequence();

        if (isShown)
        {
            anchorPointUI.localScale = Vector3.zero;
            anchorPointUI.rotation = Quaternion.Euler(0, 0, 180);
            anchorPointSequence.Join(anchorPointUI.DOScale(Vector3.one, anchorPointTransitionDuration).SetEase(Ease.OutBack));
            anchorPointSequence.Join(anchorPointUI.DORotate(Vector3.zero, anchorPointTransitionDuration).SetEase(Ease.OutBack));
        }
        else
        {
            anchorPointUI.localScale = Vector3.one;
            anchorPointUI.rotation = Quaternion.Euler(0, 0, 0);
            anchorPointSequence.Join(anchorPointUI.DOScale(Vector3.zero, anchorPointTransitionDuration).SetEase(Ease.InBack));
            anchorPointSequence.Join(anchorPointUI.DORotate(Vector3.forward * 180, anchorPointTransitionDuration).SetEase(Ease.InBack));
        }
    }
}
