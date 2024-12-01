using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropContainer : MonoBehaviour
{
    public GameObject goldPrefab;
    public GameObject expPrefab;
    public GameObject DropGold(Transform loc)
    {
        return Instantiate(goldPrefab, loc);
    }
    public GameObject DropExp(Transform loc)
    {
        return Instantiate(expPrefab, loc);
    }
    public GameObject DropComponent(TinkerComponentType type, Transform loc)
    {
        return Instantiate(TinkerComponentManager.instance.DropComponent(type, loc));
    }
    public GameObject DropWeapon(WeaponType type, Transform loc)
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
