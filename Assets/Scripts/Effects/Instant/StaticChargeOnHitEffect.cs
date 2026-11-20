using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.UI.Image;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Static Charge On Hit Effect")]
public class StaticChargeOnHitEffect : OnHitEffect
{
    public float timeToFullCharge = 1f;
    public float radius = 1f;
    public LayerMask enemyLayers;
    public override void ProcessEffect(CharacterManager target)
    {
        Debug.Log("StaticChargeOnHitEffect ProcessEffect target=" + target.name);
        if (weaponDealingDamage == null || weaponDealingDamage.staticChargeValue < timeToFullCharge)
            return; // Not yet charged
        weaponDealingDamage.staticChargeValue = 0;
        weaponDealingDamage.fullyChargedStaticCharge = false;
        Destroy(weaponDealingDamage.staticChargeFX);
        Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, radius, enemyLayers);
        foreach (Collider collider in hitColliders)
        {
            // Check for target
            if (collider.gameObject == target.gameObject) continue;
            AICharacterManager ai = collider.GetComponent<AICharacterManager>();
            if(ai == null) continue;

            // Apply effects
            Instantiate(WorldCharacterEffectsManager.StaticChargeHitFX, ai.transform);
            ai.ApplyDamage(hitDamage * 0.2f, null, false, "white");
            //Editor only visual debug line
            Debug.DrawLine(target.transform.position, ai.transform.position, Color.blue);

            //return; // This will make it only hit one enemy.
        }
    }
}
