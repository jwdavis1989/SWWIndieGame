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

    protected virtual void Start()
    {

    }

    protected virtual void Awake()
    {

    }
    
    protected virtual void Update()
    {
        
    }

}
