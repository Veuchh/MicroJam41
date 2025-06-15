using System;
using UnityEngine;

public class AnchorPoint : MonoBehaviour
{
    public static event Action OnEndAnchorPointGrabbed;

    [SerializeField]
    Sprite highlightedSprite;

    [SerializeField]
    Sprite unhighlightedSprite;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    bool isAnchorPointEndGame = false; 

    public bool IsAnchorPointEndGame => isAnchorPointEndGame;

    public Rigidbody2D RB => rb;

    private void Awake()
    {
        Highlight(false);
    }

    public void Highlight(bool highlight)
    {
        spriteRenderer.sprite = highlight ? highlightedSprite : unhighlightedSprite;
    }

    public void OnGrabbed()
    {
        if (isAnchorPointEndGame)
            OnEndAnchorPointGrabbed?.Invoke();
    }
}
