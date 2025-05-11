using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DEPRECITATED KEEPING AROUND FOR NOSTALGIA
public class AggroCollider : MonoBehaviour
{
    [Header("***   DEPRECIATED   ***")]
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
            gameObject.transform.parent.GetComponent<AiCharacterCombatManager>().AggroPlayer(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
