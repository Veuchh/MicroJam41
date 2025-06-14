using Core.Player;
using UnityEngine;

public class RocketRefillCollectible : MonoBehaviour
{
    const string PLAYER_TAG = "Player";

    [SerializeField]
    float cooldownDuration = 2f;

    [SerializeField]
    GameObject gfx;

    float cooldownEnd;
    bool isShown = false;

    public bool CanBePicked => Time.time >= cooldownEnd;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG) && CanBePicked)
        {
            other.GetComponent<PlayerRocket>()?.RefillRockets();
            ToggleCollectibleGFX(false);
            cooldownEnd = Time.time + cooldownDuration;
        }
    }

    private void Update()
    {
        if (!isShown && CanBePicked)
            ToggleCollectibleGFX(true);
    }

    void ToggleCollectibleGFX(bool toggle)
    {
        isShown = toggle;

        gfx.SetActive(toggle);
    }
}
