using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class ParallaxElementsManager : MonoBehaviour
{
    [SerializeField, MinMaxRangeSlider(-100f, 100f)]
    Vector2 minMaxPlayerXPos;

    [SerializeField, MinMaxRangeSlider(-100f, 100f)]
    Vector2 minMaxPlayerYPos;

    [SerializeField]
    List<ParallaxElement> parallaxElements;

    [SerializeField]
    Transform mainCamera;

    private void Update()
    {
        if (mainCamera == null)
            return;

        float playerXPosInterval = Mathf.InverseLerp(minMaxPlayerXPos.x, minMaxPlayerXPos.y, mainCamera.position.x);
        float playerYPosInterval = Mathf.InverseLerp(minMaxPlayerYPos.x, minMaxPlayerYPos.y, mainCamera.position.y);

        foreach (var parallaxElement in parallaxElements)
        {
            parallaxElement.UpdateParallax(playerXPosInterval, playerYPosInterval, mainCamera.position);
        }
    }
}
