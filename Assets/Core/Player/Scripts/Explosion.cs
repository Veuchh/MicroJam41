using UnityEngine;

public class Explosion : MonoBehaviour
{
    const string PLAYER_TAG = "Player";
    
    [SerializeField]
    float lifetime = .3f;
    
    [SerializeField]
    float explosionForce = 10;

    float endTime;

    private void Start()
    {
        endTime = Time.time + lifetime;
    }

    private void Update()
    {
        if (Time.time > endTime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG) && other.GetComponent<Rigidbody2D>() is Rigidbody2D otherRb)
        {
            Vector2 direction = (otherRb.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(otherRb.transform.position, transform.position);
            float forceMagnitude = explosionForce / (distance + 0.01f);
            otherRb.AddForce(direction * forceMagnitude, ForceMode2D.Impulse);
        }
    }
}
