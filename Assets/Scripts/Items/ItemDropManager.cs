using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    [SerializeField]
    public GameObject goldPrefab;
    [SerializeField]
    public GameObject expPrefab;
    [SerializeField]
    public GameObject genericItemDropPrefab;
    [SerializeField]
    public ItemDatabase itemDatabase;
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
        //ExpDropCollider e = exp.GetComponent<ExpDropCollider>();
        ExpDropCollider e = exp.GetComponentInChildren<ExpDropCollider>();
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
        GameObject weaponDrop = WeaponsController.instance.CreateWeapon(type, loc);
        return weaponDrop;
    }
    public static GameObject DropWeaponById(string itemId, Transform loc) //TODO dropped weapons pickup-able
    {
        //Warning: CreateWeapon creates object under the Transform loc which would cause the weapon to dissapear when loc (possibly a dead enemy) dissapears
        GameObject weaponDrop = WeaponsController.instance.CreateWeaponById(itemId, loc);
        return weaponDrop;
    }
    public GameObject DropItemById(string itemId, Transform loc)
    {
        if(genericItemDropPrefab == null) return null;

        GameObject obj;
        ItemDetails itemDetails = itemDatabase.GetItem(itemId);
        if (itemDetails.itemType == "component")
        {
            Debug.Log("Creating component:" + itemDetails.itemId);
            obj = TinkerComponentManager.instance.DropComponentById(itemDetails.itemId, transform);
        }
        else if (itemDetails.itemType == "weapon")
        {
            Debug.Log("Creating weapon:" + itemDetails.itemId);
            obj = DropWeaponById(itemDetails.itemType, loc);
        }
        else
        {
            Debug.Log("Creating genericItemDropPrefab:" + itemDetails.itemId);
            obj = Instantiate(genericItemDropPrefab, loc);
            obj.GetComponent<PickupableItem>().itemId = itemId;
        }
        return obj;
    }
    //singleton pattern
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
