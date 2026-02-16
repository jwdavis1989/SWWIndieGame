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
    [SerializeField] GameObject leftFootstepDustVFXAnchor;
    
    [SerializeField] GameObject rightFootstepDustVFXAnchor;

    [Header("Current Active VFX")]
    public GameObject activeSpellWarmUpFX;

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
            if (character.characterModel)
            {    
                Instantiate(deathVFX, character.characterModel.transform.position, Quaternion.identity);
            }
            //This is done for enemies whose model doesn't align with their coordinates, such as the Masterwork Fabricator/Parent Unit
            else
            {
                Instantiate(deathVFX, transform.position, Quaternion.identity);
            }
        }
        else
        {
            Instantiate(WorldCharacterEffectsManager.instance.defaultDeathExplosionVFX, transform.position, Quaternion.identity);
        }
    }

    public void ActivateFootstepVFX(bool isLeftInsteadOfRight)
    {
        if (character.isGrounded && leftFootstepDustVFXAnchor && rightFootstepDustVFXAnchor)
        {
            if (isLeftInsteadOfRight)
            {
                Instantiate(WorldCharacterEffectsManager.instance.defaultfootstepDustVFX, leftFootstepDustVFXAnchor.transform.position, leftFootstepDustVFXAnchor.transform.rotation);
            }
            else
            {
                Instantiate(WorldCharacterEffectsManager.instance.defaultfootstepDustVFX, rightFootstepDustVFXAnchor.transform.position, rightFootstepDustVFXAnchor.transform.rotation);
            }
        }
    }

}
