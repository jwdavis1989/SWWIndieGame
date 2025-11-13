using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Velocity")]
    public float forwardVelocity = 2200f;
    public float upwardVelocity = 0f;
    public float ammoMass = 0.01f;

    [Header("Damage")]
    public WeaponStats gunWeaponStats;

    [Header("Model")]
    public GameObject ProjectileModel;

    

}
