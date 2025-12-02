using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class TinkerComponentDropCollider : MonoBehaviour
{
    public TinkerComponent tinkerComponent;
    // Start is called before the first frame update
    void Start()
    {
        if(tinkerComponent == null)
        {
            tinkerComponent = GetComponentInParent<TinkerComponent>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //TODO: Play Pick Up Sound here
            if (tinkerComponent.stats.isWeapon)
            {
                //Broken down Weapon Components probably shouldn't be on the ground but handle for them anyways
                TinkerComponentManager.instance.weaponComponents.Add(gameObject);
                Destroy(gameObject);
            }
            else
            {

                //regular component
                if (tinkerComponent.stats.count <= 0) tinkerComponent.stats.count = 1; // Allow use of positive count for multiple drop in 1 item, otherwise act as a single drop
                TinkerComponentManager.instance.AddBaseComponentToPlayer(tinkerComponent.stats.componentType, tinkerComponent.stats.count);
                Destroy(gameObject);
            }
        }
    }
}
