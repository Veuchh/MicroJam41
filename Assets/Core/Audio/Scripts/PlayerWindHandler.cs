using Unity.Cinemachine;
using UnityEngine;

public class PlayerWindHandler : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    [SerializeField, MinMaxRangeSlider(0f, 50f)]
    Vector2 minMaxPlayerSpeed;

    [SerializeField, MinMaxRangeSlider(0f, 3f)]
    Vector2 minMaxAudioSourceVolume;

    [SerializeField]
    float lerpSpeed = .01f;
    
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float velocityMagnitude = rb.linearVelocity.magnitude;

        float targetAudioSourceVolume = Mathf.Lerp(
            minMaxAudioSourceVolume.x,
            minMaxAudioSourceVolume.y,
            Mathf.InverseLerp(
                minMaxPlayerSpeed.x,
                minMaxPlayerSpeed.y,
                velocityMagnitude));

        float newVolume = Mathf.Lerp(audioSource.volume, targetAudioSourceVolume, lerpSpeed * Time.deltaTime);

        audioSource.volume = newVolume;
    }
}
