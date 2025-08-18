using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    
}

public enum CharacterSlot {
    CharacterSlot_01, 
    CharacterSlot_02, 
    CharacterSlot_03, 
    CharacterSlot_04, 
    CharacterSlot_05, 
    CharacterSlot_06, 
    CharacterSlot_07, 
    CharacterSlot_08, 
    CharacterSlot_09, 
    CharacterSlot_10, 
    NO_SLOT
}

//This is used to calculate damage based on attack type
public enum AttackType
{
    LightAttack01,
    LightAttack02,
    LightAttack03,
    HeavyAttack01,
    HeavyAttack02,
    ChargedAttack01,
    ChargedAttack02,
    RunningLightAttack01,
    RunningHeavyAttack01,
    RollingLightAttack01,
    RollingHeavyAttack01,
    BackstepLightAttack01,
    BackstepHeavyAttack01,
    AreaSpellAttack01,
    HeavyJumpAttack01,
    LightJumpAttack01,
    Attack1
}

//Intensity of blocked attacks
public enum DamageIntensity
{
    Ping,
    Light,
    Medium,
    Heavy,
    Colossal
}

//Factions to determine who will/can attack who
public enum CharacterFaction
{
    TeamPlayer, //Player, Pets, Summons
    TeamGreen, //Player cannot attack
    TeamYellow, //Non-Hostile, Attackable by player
    TeamHostile01, //Hostile to player, TeamHostile02, and TeamHostile03
    TeamHostile02, //Hostile to yellow, player, TeamHostile01, and TeamHostile02
    TeamHostile03, //Hostile to yellow, player, TeamHostile01, and TeamHostile02
}