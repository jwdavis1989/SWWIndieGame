using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;
    //Using instead of a network variable
    public string characterName = "Character Name";
    //gold
    public BigInteger gold = 0;

    protected override void Awake() {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Start() {
        base.Start();

        //Called here in tutorial, but likely obsolete with other changes made in our version
            // CalculateHealthBasedOnfortitudeLevel(fortitude);
            // CalculateStaminaBasedOnEnduranceLevel(endurance);
    }

    public void FullyRestoreResources() {
        SetNewMaxHealthValue();
        SetNewMaxStaminaValue();
        if (player.isDead) {
            player.ReviveCharacter();
        }
    }
}
