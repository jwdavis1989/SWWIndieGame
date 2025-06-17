using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapRevealCollider : MonoBehaviour
{
    //private Collider miniMapRevealCollider;
    private GameObject miniMapTile;

    // Start is called before the first frame update
    void Start()
    {
        //miniMapRevealCollider = GetComponent<Collider>();
        miniMapTile = transform.GetChild(0).gameObject;
        miniMapTile.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        miniMapTile.SetActive(true);
    }
}
