using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLoot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InteractableChestSimple chest = GetComponent<InteractableChestSimple>();
        if(chest != null)
        {
            GameObject randomComponent = TinkerComponentManager.instance.DropRandomItem(transform);
            chest.contents.Add(randomComponent);
            randomComponent.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
