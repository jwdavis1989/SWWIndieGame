using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgradeManager : MonoBehaviour
{
    /**
     * CanUseComponent will return true if any stat will be upgraded. I.e. if any matching stat is not currently maxed
     */
    public bool CanUseComponent(WeaponScript weapon, TinkerComponent tinkerComponent)
    {
        return AddTinkerComponentToWeapon(weapon, tinkerComponent, false);
    }
    public bool UseComponent(WeaponScript weapon, TinkerComponent tinkerComponent)
    {
        return AddTinkerComponentToWeapon(weapon, tinkerComponent, true);
    }
    public bool AddTinkerComponentToWeapon(WeaponScript weapon, TinkerComponent tinkerComponent, bool doUpdate)
    {
        bool canUpgrade = false;
        float newAttack = weapon.stats.attack;
        //calulate new attack
        if (tinkerComponent.isWeapon)
        {
            if (weapon.stats.attack < tinkerComponent.attack)
            {
                newAttack += tinkerComponent.attack * 0.25f;
            }
            else
            {
                newAttack += tinkerComponent.attack * 0.60f;
            }
        }
        else
        {
            newAttack += tinkerComponent.attack;
        }
        newAttack = Mathf.Min(newAttack, weapon.stats.maxAttack);
        //calculate new elemental
        ElementalStats newStats = weapon.stats.elemental.Add(tinkerComponent.elementalStats);
        newStats.firePower = Mathf.Min(newStats.firePower, weapon.stats.maxElemental.firePower);
        newStats.icePower = Mathf.Min(newStats.icePower, weapon.stats.maxElemental.icePower);
        newStats.lightningPower = Mathf.Min(newStats.lightningPower, weapon.stats.maxElemental.lightningPower);
        newStats.windPower = Mathf.Min(newStats.windPower, weapon.stats.maxElemental.windPower);
        newStats.earthPower = Mathf.Min(newStats.earthPower, weapon.stats.maxElemental.earthPower);
        newStats.lightPower = Mathf.Min(newStats.lightPower, weapon.stats.maxElemental.lightPower);
        newStats.beastPower = Mathf.Min(newStats.beastPower, weapon.stats.maxElemental.beastPower);
        newStats.scalesPower = Mathf.Min(newStats.scalesPower, weapon.stats.maxElemental.scalesPower);
        newStats.techPower = Mathf.Min(newStats.techPower, weapon.stats.maxElemental.techPower);
        ElementalStats diffWithPrev = newStats.CalculateElementDiff(weapon.stats.elemental);
        //if any stat will be upgraded then we can upgrade
        if (diffWithPrev.firePower > 0 ||
            diffWithPrev.icePower > 0 ||
            diffWithPrev.lightningPower > 0 ||
            diffWithPrev.windPower > 0 ||
            diffWithPrev.earthPower > 0 ||
            diffWithPrev.lightPower > 0 ||
            diffWithPrev.beastPower > 0 ||
            diffWithPrev.scalesPower > 0 ||
            diffWithPrev.techPower > 0 ||
            weapon.stats.attack < newAttack)
        {
            canUpgrade = true;
            if (doUpdate)
            {
                weapon.stats.elemental = newStats;
                weapon.stats.attack = newAttack;
                tinkerComponent.count--;
            }
        }
        return canUpgrade;
    }
}
