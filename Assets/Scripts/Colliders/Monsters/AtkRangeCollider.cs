using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkRangeCollider : MonoBehaviour
{
    private SphereCollider atkCollider;
    void Awake()
    {
        atkCollider = GetComponent<SphereCollider>();
    }
    public void SetRange(float range)
    {
        atkCollider.radius = range;
    }
    public void OnTriggerEnter(Collider other)
    {
        //if(other.gameObject == gameObject.transform.parent.GetComponent<CharacterCombatManager>().currentTarget) {
        if (other.CompareTag("Player"))
        {
            gameObject.transform.parent.GetComponent<EnemyManager>().BeginAttack01();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
