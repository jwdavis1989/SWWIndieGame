using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortswordScript : WeaponScript
{
    [Header("Weapon Attributes")]
    new public float attack = 15.0f;
    new public int maxDurability = 100;
    new public float block = 20.0f;
    new public float stability = 0.0f;
    new public int firePower = 0;
    new public int icePower = 0;
    new public int lightningPower = 0;
    new public int windPower = 0;
    new public int earthPower = 10;
    new public int lightPower = 0;
    new public int beastPower = 10;
    new public int scalesPower = 0;
    new public int techPower = 0;
    new public int tinkerPointsPerLvl = 1;

    new public float speed = 1.0f;
    
    [Header("Weapon State")]
    new public float currentDurability = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
