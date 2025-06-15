using UnityEngine;

public class ImpactDetection : MonoBehaviour
{
    const string PLATFORM_TAG = "Platform";

    [SerializeField]
    float minVelocityThresholdToTrigger = 20;

    [SerializeField] GameObject dustCloud;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(PLATFORM_TAG)
            && collision.relativeVelocity.magnitude > minVelocityThresholdToTrigger)
        {
            Instantiate(dustCloud, collision.contacts[0].point, 
                Quaternion.Euler(
                    0,
                    0,
                    Vector2.SignedAngle(Vector2.up, collision.contacts[0].normal)));
        }
    }
}
