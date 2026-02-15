using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLargeChestLoot : MonoBehaviour
{
    public int floor = 1;
    // Start is called before the first frame update
    void Start()
    {
        InteractableChestSimple chest = GetComponent<InteractableChestSimple>();
        if (chest != null)
        {
            //add random component
            //GameObject randomComponent = TinkerComponentManager.instance.DropRandomGem(transform);
            //chest.contents.Add(randomComponent);
            //randomComponent.SetActive(false);

            GameObject itemToAdd;
            float gemDropChance = 35f;
            float randomNumber = Random.Range(0f, 100f);
            //float weaponDropChance = 32.5f;

            if (floor < 6)
                gemDropChance = 80f;

            if (floor >= 6 && !JournalManager.instance.CheckJournalFlag(JournalManager.hasObtainedTier2RareWeapon))
            { // first Floor 6+ Large Chest. Guaranteed weapon.
                gemDropChance = -1f;
                if (randomNumber > 50f)
                { // drop Bone Scimitar
                    itemToAdd = ItemDropManager.DropWeapon(WeaponType.BoneBlade, transform);
                }
                else
                { // drop Frost Wand
                    itemToAdd = ItemDropManager.DropWeapon(WeaponType.BoneBlade, transform);
                }
            }

            if (randomNumber < gemDropChance)
            {
                itemToAdd = TinkerComponentManager.instance.DropRandomGem(transform);
            }
            else
            {
                //drop weapon
                if (randomNumber > 50f) // drop Bone Scimitar
                    itemToAdd = ItemDropManager.DropWeapon(WeaponType.BoneBlade, transform);
                else // drop Frost Wand
                    itemToAdd = ItemDropManager.DropWeapon(WeaponType.BoneBlade, transform);
            }
            chest.contents.Add(itemToAdd);
            itemToAdd.SetActive(false);
        }
    }
}
