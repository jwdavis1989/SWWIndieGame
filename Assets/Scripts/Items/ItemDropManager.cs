using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    [SerializeField]
    public GameObject goldPrefab;
    [SerializeField]
    public GameObject expPrefab;
    public GameObject DropGold(Transform loc, int amt)
    {
        GameObject g = Instantiate(goldPrefab, loc.position, loc.rotation);
        g.GetComponentInChildren<GoldDropCollider>().gold = amt;
        return g;
    }
    public GameObject DropExp(Transform loc, int amt, bool giveMainHandExp, bool giveOffHandExp)
    {
        if (!giveMainHandExp && !giveOffHandExp)
        {
            Debug.Log("Warning: Main/Off Hand Not Set! Giving to both.");
            giveMainHandExp = giveOffHandExp = true;
        }
        GameObject exp = Instantiate(expPrefab, loc.position, loc.rotation);
        ExpDropCollider e = exp.GetComponent<ExpDropCollider>();
        //ExpDropCollider e = exp.GetComponentInChildren<ExpDropCollider>();
        e.exp = amt;
        e.isMainHandExp = giveMainHandExp;
        e.isOffHandExp = giveOffHandExp;
        return exp;
    }
    public static GameObject DropComponent(TinkerComponentType type, Transform loc)
    {
        return TinkerComponentManager.instance.DropComponent(type, loc);
    }
    public static GameObject DropWeapon(WeaponType type, Transform loc) //TODO dropped weapons pickup-able
    {
        //Warning: CreateWeapon creates object under the Transform loc which would cause the weapon to dissapear when loc (possibly a dead enemy) dissapears
        return Instantiate(WeaponsController.instance.CreateWeapon(type, loc));
    }
    public static ItemDropManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
