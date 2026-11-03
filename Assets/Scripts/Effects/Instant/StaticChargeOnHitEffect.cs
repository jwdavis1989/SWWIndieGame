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
    public GameObject staticChargeVFX;
    public override void ProcessEffect(CharacterManager target)
    {
        if (weaponDealingDamage == null || weaponDealingDamage.staticChargeValue < timeToFullCharge)
            return; // Not yet charged
        weaponDealingDamage.staticChargeValue = 0;
        Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, radius, enemyLayers);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject == target.gameObject) continue;

            AICharacterManager ai = collider.GetComponent<AICharacterManager>();
            if(ai == null) continue;

            //VFX
            if (staticChargeVFX != null)
                Instantiate(staticChargeVFX, target.transform);

            //deal damage
            ai.ApplyDamage(hitDamage * 0.2f, null, false, "white");

            return; // Putting this here so it only hits one enemy.
        }
    }
}
