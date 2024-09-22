using Palmmedia.ReportGenerator.Core.Common;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponType
{
    Shortsword,
    Wrench,
    BastardSword,
    BroadSword,
    BoneBlade,
    ReinforcedWrench,
    //specialty weapons
    Dagger
}
[Serializable]
public class WeaponsArray
{
    public BaseWeaponStats[] weapons;
}
[Serializable]
public class BaseWeaponStats
{
    public float attack = 1.0f;
    public float speed = 1.0f;
    public float specialtyCooldown = 0;
    public float block = 1.0f;
    public float stability = 1.0f;
    public float xpToLevel = 100.0f;
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
    public WeaponType weaponType = 0;
    public float currentDurability = 1.0f;
    public int level = 1;
    public float currentExperiencePoints = 0.0f;
    public int currentTinkerPoints = 0;
    public String weaponName = "BaseWeaponName";
}


    public class WeaponsController : MonoBehaviour
{

    public GameObject[] weapons;
    public TextAsset jsonFile;
    // Start is called before the first frame update
    void Start()
    {
        //read json file and create objects
        WeaponsArray weaponsJsons = JsonUtility.FromJson<WeaponsArray>(jsonFile.text);
        GameObject[] weapons2 = new GameObject[Enum.GetValues(typeof(WeaponType)).Cast<int>().Max()];
        foreach(BaseWeaponStats weaponStat in weaponsJsons.weapons)
        {
            int i = (int)weaponStat.weaponType;
            weapons2[i] = new GameObject("Empty");
            weapons2[i].AddComponent(typeof(BaseWeaponScript));
            weapons2[i].GetComponent<BaseWeaponScript>().copy(weaponStat);
            Debug.Log("Weapon " + i + ": WeaponType:" + weapons2[i].GetComponent<BaseWeaponScript>().weaponType 
                + " Atk:"+ weapons2[i].GetComponent<BaseWeaponScript>().attack); //astest
        }
        weapons = weapons2;
        Debug.Log("Attack of level 1 BastardSword:" + GetWeaponAttack(WeaponType.BastardSword, 1));//astest
    }
    /**
     * Perhaps could use a method to calculate the attributes of a weapon of a particular level
     */
    public float GetWeaponAttack(WeaponType type, int level)
    {
        return weapons[(int)type].GetComponent<BaseWeaponScript>().attack * level;
    }
}
/** Change Log  
 *  Date         Developer  Description
 *  09/16/2024   Alec       New.
 *  
 * */