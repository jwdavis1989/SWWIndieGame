using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RareLootDisplay : MonoBehaviour
{
    string lootItemId = "bone_scimitar";
    // Start is called before the first frame update
    void Start()
    {
        // raise
        Vector3 position = transform.position;
        position.y += 2f;
        transform.position = position;
        // rotate
        Vector3 rotation = new Vector3(0,90f,0);
        transform.rotation = Quaternion.Euler(rotation);
        // start timer
        StartCoroutine(WaitThenGiveToPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        // could make item spin here
    }
    IEnumerator WaitThenGiveToPlayer()
    {
        // wait
        yield return new WaitForSeconds(3);
        // give to player
        PlayerWeaponManager.instance.AddWeaponById(lootItemId);
        // end this object
        Destroy(gameObject);
    }
}
