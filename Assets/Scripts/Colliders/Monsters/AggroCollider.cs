using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroCollider : MonoBehaviour
{
    private SphereCollider aggroCollider;
    void Awake()
    {
        aggroCollider = GetComponent<SphereCollider>();
    }
    public void SetRange(float range)
    {
        aggroCollider.radius = range;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.transform.parent.GetComponent<AICharacterManager>().AggroPlayer(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
