using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
[CreateAssetMenu(menuName = "Character Effects/Timed Effects/Slow Movement")]
public class SlowMovementEffect : TimedCharacterEffect
{
    public float movementMultipler;
    public override void OnEffectStart(CharacterManager character)
    {
        base.OnEffectStart(character);
        if (character.isPlayer)
        {
            Debug.Log("Adding slow effect to player");
            PlayerLocomotionManager locomotionManager = character.GetComponent<PlayerLocomotionManager>();
            locomotionManager.AddSpeedModifier(effectId, movementMultipler);
        }
        else
        { /* AI Character */
            Debug.Log("Adding slow effect to AI:" + character.name);
            AiCharacterCombatManager combatManager = character.GetComponent<AiCharacterCombatManager>();
            combatManager.AddSpeedModifier(effectId, movementMultipler);
            character.animator.speed = combatManager.GetBasicMovementSpeed();
        }
    }
    public override void OnEffectFinish(CharacterManager character)
    {
        base.OnEffectFinish(character);
        Debug.Log("removing slow effect");
        if (character.isPlayer)
        {
            PlayerLocomotionManager locomotionManager = character.GetComponent<PlayerLocomotionManager>();
            locomotionManager.RemoveSpeedModifer(effectId);

        }
        else
        { // AI Character
            AiCharacterCombatManager combatManager = character.GetComponent<AiCharacterCombatManager>();
            combatManager.RemoveSpeedModifer(effectId);
            character.animator.speed = combatManager.GetBasicMovementSpeed();
        }
    }
    public override ActiveCharacterEffect ActiveEffect()
    {
        return base.ActiveEffect();
    }
}
