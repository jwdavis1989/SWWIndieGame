using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropContainer : MonoBehaviour
{
    public GameObject goldPrefab;
    public GameObject expPrefab;
    public GameObject DropGold(Transform loc, int amt)
    {
        GameObject g = Instantiate(goldPrefab, loc);
        g.GetComponent<GoldDropCollider>().gold = amt;
        return g;
    }
    public GameObject DropExp(Transform loc, int amt, bool isHitByMainHand, bool isHitByOffHand)
    {
        GameObject exp = Instantiate(expPrefab, loc);
        ExpDropCollider e = exp.GetComponent<ExpDropCollider>();
        e.exp = amt;
        e.isMainHandExp = isHitByMainHand;
        e.isOffHandExp = isHitByOffHand;
        return exp;
    }
    public GameObject DropComponent(TinkerComponentType type, Transform loc)
    {
        return Instantiate(TinkerComponentManager.instance.DropComponent(type, loc));
    }
    public GameObject DropWeapon(WeaponType type, Transform loc) //TODO dropped weapons pickup-able
    {
        return Instantiate(WeaponsController.instance.CreateWeapon(type, loc));
    }
    public static ItemDropContainer instance;
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
