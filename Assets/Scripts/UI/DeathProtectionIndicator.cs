using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DeathProtectionIndicator : MonoBehaviour
{
    [SerializeField] private Image deathProtectionSprite;
    [SerializeField] private Image availableSprite;

    private void OnEnable()
    {
        EventBus.Subscribe<DeathProtectionStateChangedEvent>(OnDeathProtectionStateChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<DeathProtectionStateChangedEvent>(OnDeathProtectionStateChanged);
    }

    private void OnDeathProtectionStateChanged(DeathProtectionStateChangedEvent eventData)
    {
        deathProtectionSprite.enabled = eventData.isUnlocked;
        availableSprite.enabled = !eventData.isAvailable && eventData.isUnlocked;
    }

}
