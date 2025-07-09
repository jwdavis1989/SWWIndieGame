using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    //Process Instant Effects (e.g. Take Damage, Healing)

    //Process Timed Effects (Poison, Builds-up)

    //Process Static Effects (Adding/Removing Buffs/Debuffs from equipped items)

    CharacterManager character;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplatterVFX;
    [SerializeField] GameObject deathVFX;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        //Take in an effect
        //Process it
        effect.ProcessEffect(character);
    }

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        //If we manually have placed a blood splatter VFX on this model, play its version
        if (bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
        //Else we use the generic version we have elsewhere
        else
        {
            if (character.canBleed)
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.defaultBloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }
    }

    public void PlayDeathVFX()
    {
        if (deathVFX != null)
        {
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(WorldCharacterEffectsManager.instance.defaultDeathExplosionVFX, transform.position, Quaternion.identity);
        }
    }

}
