using UnityEngine;

public class DestroyOnLevelEnd : MonoBehaviour
{
    private void Awake()
    {
        AnchorPoint.OnEndAnchorPointGrabbed += OnLevelEnd;
    }

    private void OnDestroy()
    {
        AnchorPoint.OnEndAnchorPointGrabbed -= OnLevelEnd;
    }

    void OnLevelEnd()
    {
        Destroy(gameObject);
    }
}
