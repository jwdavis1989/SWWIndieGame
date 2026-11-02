using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightInterferenceCollider : MonoBehaviour
{
    [Header("Collider")]
    public Collider effectCollider;

    // Start is called before the first frame update
    void Awake()
    {
        if (effectCollider == null)
        {
            effectCollider = GetComponent<Collider>();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            PlayerManager colliderTarget = other.GetComponentInParent<PlayerManager>();
            if (colliderTarget != null && !colliderTarget.isRunningOnEmergencyPowerLevels)
            {
                colliderTarget.playerStatsManager.HandleFlashlightFlicker(true);
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            PlayerManager colliderTarget = other.GetComponentInParent<PlayerManager>();
            if (colliderTarget != null && !colliderTarget.isRunningOnEmergencyPowerLevels)
            {
                colliderTarget.playerStatsManager.HandleFlashlightFlicker(false);
                colliderTarget.canUseFlashlight = true;
            }
        }
    }
}
