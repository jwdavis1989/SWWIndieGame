using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    public static WorldCharacterEffectsManager instance;
    [Header("VFX")]
    public GameObject defaultBloodSplatterVFX;
    public GameObject defaultDeathExplosionVFX;
    public GameObject defaultfootstepDustVFX;

    [Header("Damage")]
    public TakeHealthDamageCharacterEffect takeHealthDamageEffect;
    public TakeBlockedHealthDamageCharacterEffect takeBlockedHealthDamageCharacterEffect;
    [SerializeField] List<InstantCharacterEffect> instantEffects;

    private void Awake() {
        if (instance == null) {
            Debug.Log("Creating WorldCharacterEffectsManager " + gameObject.name);
            instance = this;
            DontDestroyOnLoad(gameObject);
            WorldUtilityManager.StaticObjects.Add(gameObject);
        }
        else
        {
            Debug.Log("Extra WorldCharacterEffectsManager " + gameObject.name);
            Destroy(gameObject);
        }

        GenerateEffectIDs();
    }
    private void OnDestroy()
    {
        Debug.Log("Destroy WorldCharacterEffectsManager " + gameObject.name);
        instance = null; // For main menu button
    }

    private void GenerateEffectIDs() {
        for (int i = 0; i < instantEffects.Count; i++) {
            instantEffects[i].instantEffectID = i;
        }
    }
}
