using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndScreen : MonoBehaviour
{
    [SerializeField]
    float initialDelay = 1f;

    [SerializeField]
    float blackBGFadeInDuration = .2f; 
    [SerializeField]
    CanvasGroup blackBG;

    [SerializeField]
    Transform textPannel;

    [SerializeField]
    float textPannelTweenDuration = .75f;
    
    [SerializeField]
    Button returnButton;

    [SerializeField, Scene]
    string mainMenuScene;

    private void Awake()
    {
        AnchorPoint.OnEndAnchorPointGrabbed += ShowGameEndScreen;

        returnButton.onClick.AddListener(OnReturnButtonClicked);
    }

    private void OnDestroy()
    {
        AnchorPoint.OnEndAnchorPointGrabbed -= ShowGameEndScreen;
        returnButton.onClick.RemoveListener(OnReturnButtonClicked);
    }

    public void ShowGameEndScreen()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(initialDelay);
        sequence.Append(blackBG.DOFade(1, blackBGFadeInDuration));
        sequence.Append(textPannel.DOLocalMoveY(0, textPannelTweenDuration).SetEase(Ease.OutBack));

    }

    void OnReturnButtonClicked()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
