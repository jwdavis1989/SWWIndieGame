using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    public Scrollbar loadingBar;
    void Start()
    {
        // Disable Player Gravity to avoid infinite falling bug
        TeleportData.playerManager.hasGravity = false;

        StartCoroutine(LoadScene());
    }
    /** Will use string name of scene if not null else uses index */
    IEnumerator LoadScene()
    {
        AsyncOperation operation;
        if(TeleportData.SceneIdString != null)
            operation = SceneManager.LoadSceneAsync(TeleportData.SceneIdString);
        else
            operation  = SceneManager.LoadSceneAsync(TeleportData.SceneID);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            // update loading bar
            loadingBar.size = operation.progress;
            yield return null;
        }
        operation.allowSceneActivation = true;

        // Enable Controls
        if (TeleportData.enableAfterLoad)
        {
            PlayerInputManager.instance.SafeEnable();
            Time.timeScale = 1;

            // Re-enable Player Gravity
            TeleportData.playerManager.hasGravity = true;
        }
        // Teleport
        TeleportData.playerManager.transform.position = TeleportData.Destination;
    }
}
public static class TeleportData
{
    public static int SceneID;
    public static string SceneIdString;
    public static Vector3 Destination;
    public static PlayerManager playerManager;
    public static bool enableAfterLoad = true;
}