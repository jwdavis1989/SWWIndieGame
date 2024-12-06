using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkRangeCollider : MonoBehaviour
{
    private SphereCollider atkCollider;
    [Header("Could use to differentiate different attacks with different ranges")]
    public int atkIndex = 0;
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
            EnemyManager attaker = gameObject.transform.parent.GetComponent<EnemyManager>();
            switch (atkIndex)
            {
                case 0: attaker.BeginAttack01(); break;
                default: attaker.BeginAttack01(); break;
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
