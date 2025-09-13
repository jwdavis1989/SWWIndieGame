using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapRevealCollider : MonoBehaviour
{
    private Collider miniMapRevealCollider;
    private GameObject miniMapTile;
    //[Header("Reveals all enemy locations. ")]
    //public bool roseQuartzReveal = false; //Enemies dont use reveal collider
    [Header("Reveal all water sources, locked doors, and treasure chests.")]
    public bool journalReveal = false;

    // Start is called before the first frame update
    void Start()
    {
        miniMapRevealCollider = GetComponent<Collider>();
        miniMapTile = transform.GetChild(0).gameObject;
        miniMapTile.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Reveal();
    }
    public void Reveal()
    {
        miniMapTile.SetActive(true);
        miniMapRevealCollider.enabled = false;
    }
}
