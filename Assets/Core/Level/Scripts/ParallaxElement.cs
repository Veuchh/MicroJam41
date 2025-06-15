using NaughtyAttributes;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class ParallaxElement : MonoBehaviour
{
    [SerializeField, MinMaxRangeSlider(-100f, 100f)]
    Vector2 minMaxXOffset;

    [SerializeField, MinMaxRangeSlider(-100f, 100f)]
    Vector2 minMaxYOffset;

    [SerializeField]
    Vector2 defaultOffset;

    public void UpdateParallax(float playerXOffsetInterval, float playerYOffsetInterval, Vector3 camPos)
    {
        float newXOffset = Mathf.Lerp(minMaxXOffset.y, minMaxXOffset.x, playerXOffsetInterval);
        float newYOffset = Mathf.Lerp(minMaxYOffset.y, minMaxYOffset.x, playerYOffsetInterval);

        transform.position = new Vector3(camPos.x, camPos.y, 0) + new Vector3(newXOffset, newYOffset, 0) + new Vector3(defaultOffset.x, defaultOffset.y, 0);
    }

#if UNITY_EDITOR
    [Button]
    void RecordMinXOffset()
    {
        minMaxXOffset.x = transform.position.x;
    }

    [Button]
    void RecordMaxXOffset()
    {
        minMaxXOffset.y = transform.position.x;
    }

    [Button]
    void RecordMinYOffset()
    {
        minMaxYOffset.x = transform.position.y;
    }

    [Button]
    void RecordMaxYOffset()
    {
        minMaxYOffset.y = transform.position.y;
    }
#endif
}
