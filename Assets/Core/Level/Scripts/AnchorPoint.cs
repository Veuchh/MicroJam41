using System;
using UnityEngine;

public class AnchorPoint : MonoBehaviour
{
    [SerializeField]
    Sprite highlightedSprite;

    [SerializeField]
    Sprite unhighlightedSprite;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Rigidbody2D rb;

    public Rigidbody2D RB => rb;

    private void Awake()
    {
        Highlight(false);
    }

    public void Highlight(bool highlight)
    {
        spriteRenderer.sprite = highlight ? highlightedSprite : unhighlightedSprite;
    }
}
