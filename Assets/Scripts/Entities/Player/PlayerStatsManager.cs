using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;
    //Using instead of a network variable
    public string characterName = "Character Name";

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
}
