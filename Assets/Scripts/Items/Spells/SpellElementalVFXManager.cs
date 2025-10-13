using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellElementalVFXManager : MonoBehaviour
{
    public GameObject fireSpellVFX;
    public GameObject iceSpellVFX;
    public GameObject lightningSpellVFX;
    public GameObject windSpellVFX;
    public GameObject earthSpellVFX;
    public GameObject lightSpellVFX;
    public GameObject beastSpellVFX;
    public GameObject scalesSpellVFX;
    public GameObject techSpellVFX;

    public void ChangeVFXBasedOnElement(ElementalDamageType spellElementType)
    {
        switch (spellElementType)
        {
            case ElementalDamageType.Fire:
                fireSpellVFX.SetActive(true);
                break;
            case ElementalDamageType.Ice:
                iceSpellVFX.SetActive(true);
                break;
            case ElementalDamageType.Lightning:
                lightningSpellVFX.SetActive(true);
                break;
            case ElementalDamageType.Wind:
                windSpellVFX.SetActive(true);
                break;
            case ElementalDamageType.Earth:
                earthSpellVFX.SetActive(true);
                break;
            case ElementalDamageType.Light:
                lightSpellVFX.SetActive(true);
                break;
            case ElementalDamageType.Beast:
                beastSpellVFX.SetActive(true);
                break;
            case ElementalDamageType.Scales:
                scalesSpellVFX.SetActive(true);
                break;
            case ElementalDamageType.Tech:
                techSpellVFX.SetActive(true);
                break;
            case ElementalDamageType.Unaspected:
                fireSpellVFX.SetActive(true);
                break;
        }
    }
}
