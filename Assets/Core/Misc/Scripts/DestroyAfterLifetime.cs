using UnityEngine;

public class DestroyAfterLifetime : MonoBehaviour
{
    [SerializeField] float lifetime = 1f;

    float endLifetime;

    void Awake()
    {
        endLifetime = Time.time + lifetime;
    }

    void Update()
    {
        if (Time.time >= endLifetime)
        {
            Destroy(gameObject);
        }
    }
}
