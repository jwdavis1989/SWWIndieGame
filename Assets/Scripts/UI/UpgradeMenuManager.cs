using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuManager : MonoBehaviour
{
    public GameObject grid;
    private GridLayoutGroup gridLayoutGroup;
    public Object genericIcon;
    // Start is called before the first frame update
    void Start()
    {
        gridLayoutGroup = grid.GetComponent<GridLayoutGroup>();

    }

    // Update is called once per frame
    void Update()
    {
        //grid.AddComponent<Image>();
    }
    private void OnEnable()
    {
        foreach (Transform child in grid.transform)
        {
            Destroy(child.gameObject);
        }
        //basic components
        foreach (GameObject component in TinkerComponentManager.instance.baseComponents)
        {
            if (component == null) continue;
            TinkerComponent componentScript = component.GetComponent<TinkerComponent>();
            if (componentScript.stats.count > 0)
            {
                Object gridElement = Instantiate(genericIcon, grid.transform);
                gridElement.GetComponent<GridElementController>().topText.text = componentScript.stats.itemName;
                gridElement.GetComponent<GridElementController>().bottomText.text = "" + componentScript.stats.count;
            }
            //gridElement.GetComponent<GridElementController>().image = componentScript.;
        }
        //weapon components
        foreach(GameObject component in TinkerComponentManager.instance.weaponComponents)
        {
            TinkerComponent componentScript = component.GetComponent<TinkerComponent>();
            if (componentScript.stats.count >= 0)
            {
                Object gridElement = Instantiate(genericIcon, grid.transform);
                gridElement.GetComponent<GridElementController>().topText.text = componentScript.stats.itemName;
                gridElement.GetComponent<GridElementController>().bottomText.text = "Atk:" + componentScript.stats.attack;
            }
        }
    }
}
