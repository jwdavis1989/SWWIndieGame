using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [Header("Weapon Attributes")]
    public float attack = 1.0f;
    public float speed = 1.0f;
    public float specialtyCooldown = 0;
    public float block = 1.0f;
    public float stability = 1.0f;
    public float[] xpToLevel;
    public int maxDurability = 1;
    public int firePower = 0;
    public int icePower = 0;
    public int lightningPower = 0;
    public int windPower = 0;
    public int earthPower = 0;
    public int lightPower = 0;
    public int beastPower = 0;
    public int scalesPower = 0;
    public int techPower = 0;
    public int tinkerPointsPerLvl = 0;
    [Header("Weapon State")]
    public float currentDurability = 1.0f;
    public int level = 1;
    public float currentExperiencePoints = 0.0f;
    public int currentTinkerPoints = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void attackTarget(GameObject target)
    {
        if (target != null) {
            //target.GetComponent<EnemyController>().hp -= attack;
            //TODO
            //play weapon animation
            //set reload/recharge
        }
    }
    int[] elementList = new int[0];
    private float calculateElementalDamage(float attack, Collider other)
    {
        float result = 0;
        foreach(int element in elementList) {
            //result += attack * (element * 0.005) * (1 - other.elementalDefense(element));
        }
        if(result < 0) {
            return result;
        }
        else return 0;
    }
    int[] antiTypeList = new int[0];
    private float calculateAntiTypeDamage(float attack, Collider other)
    {
        float result = 0;
        foreach(int antiType in antiTypeList) {
            //result += attack * (antiType * 0.005) * (1 - other.antiTypeDefense(antiType));
        }
        return result;
    }

    }
