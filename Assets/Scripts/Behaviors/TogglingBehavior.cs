using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglingBehavior : MonoBehaviour
{
    [Header("TogglingBehavior allows one object to activate/deactivate others")]
    public List<GameObject> gameObjects;
    [Header("inactivateOnStart is prioritized of activateOnStart" +
        "\nBoth can be set to false")]
    public bool inactivateOnStart = false;
    public bool activateOnStart = false;
    // Start is called before the first frame update
    void Start()
    {
        if(inactivateOnStart) 
            foreach(GameObject go in gameObjects) 
                go.SetActive(false);
        else if(activateOnStart)
            foreach (GameObject go in gameObjects)
                go.SetActive(true);
    }
    public List<GameObject> Toggle(bool value)
    {
        foreach(GameObject go in gameObjects)
            go.SetActive(value);
        return gameObjects;
    }
}
