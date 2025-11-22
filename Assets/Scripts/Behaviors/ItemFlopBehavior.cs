using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlopBehavior : MonoBehaviour
{
    public GameObject item;
    // Start is called before the first frame update
    void Start()
    {
        if (item == null)
            item = gameObject;
        Rigidbody rb = item.GetComponent<Rigidbody>();
        float force = 5f;          // tune this
        Vector2 dir = new Vector2(1f, 1f).normalized; // 45° direction (right + up)
        rb.AddForce(dir * force, ForceMode.Impulse);
    }
}
