using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    [Header("Spell Target")]
    [SerializeField] protected CharacterManager spellTargetCharacter;

    [Header("VFX")]
    [SerializeField] protected GameObject impactParticleVFX;
    [SerializeField] protected GameObject impactParticleFullChargeVFX;
    protected GameObject instantiatedDestructionFX;
    public ElementalDamageType highestElementalDamageType;
    public bool isFullyCharged = false;

    protected virtual void Start()
    {

    }

    protected virtual void Awake()
    {

    }

    protected virtual void Update()
    {

    }

    public virtual void InstantiateSpellDestructionFX()
    {
        if (isFullyCharged)
        {
            instantiatedDestructionFX = Instantiate(impactParticleFullChargeVFX, transform.position, Quaternion.identity);
        }
        else
        {
            instantiatedDestructionFX = Instantiate(impactParticleVFX, transform.position, Quaternion.identity);
        }

        //Update Explosion VFX based on highest element of the magic weapon used to cast it
        instantiatedDestructionFX.GetComponent<SpellElementalVFXManager>().ChangeVFXBasedOnElement(highestElementalDamageType);
        Destroy(gameObject);
    }

    protected IEnumerator WaitThenInstantiateFX(float timeToWaitInSeconds)
    {
        yield return new WaitForSeconds(timeToWaitInSeconds);

        InstantiateSpellDestructionFX();
    }

    protected IEnumerator DestroyAfterTime(float lifeSpanInSeconds)
    {
        yield return new WaitForSeconds(lifeSpanInSeconds);

        InstantiateSpellDestructionFX();
    }

}
