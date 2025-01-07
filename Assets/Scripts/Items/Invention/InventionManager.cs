using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventionManager : MonoBehaviour
{
    public Invention[] allInventions;
    //TODO - Handle saving and loading of inventions
    //TODO - Handle capturing and saving of ideas
    //TODO - Create UI for using Ideas to make inventions
    public static InventionManager instance;
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
    public bool CheckHasUpgrade(Invention invention)
    {
        foreach (var item in allInventions) 
            if (item.type == invention.type) return true;
        return false;
    }
}
