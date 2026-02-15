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
            float randomNumber = Random.Range(0f, 100f);
            //add random component
            GameObject randomComponent = TinkerComponentManager.instance.DropRandomItem(transform);
            chest.contents.Add(randomComponent);
            randomComponent.SetActive(false);
            if(randomNumber > 50f)
            {

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
