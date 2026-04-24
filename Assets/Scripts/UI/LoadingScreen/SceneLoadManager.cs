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
        TeleportData.playerManager.transform.rotation = Quaternion.Euler(new Vector3(0,TeleportData.yRotation,0));
        FaceCameraForward(PlayerCamera.instance, TeleportData.playerManager);
    }
    void FaceCameraForward(PlayerCamera camera, PlayerManager player)
    {
        //This rotates this gameObject
        Vector3 lookPosition = player.transform.position + (Vector3.forward*5.0f);
        Debug.Log("playerPosition:" + player.transform.position);
        Debug.Log("lookPosition:"+lookPosition);
        Vector3 rotationDirection = lookPosition - camera.transform.position;
        rotationDirection.Normalize();
        rotationDirection.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, targetRotation, 0.25f);

        //This rotates the pivot object
        //We don't set rotationDirection.y = 0 because this is the up/down rotation
        rotationDirection = lookPosition - camera.cameraPivotTransform.position;
        rotationDirection.Normalize();

        targetRotation = Quaternion.LookRotation(rotationDirection);
        camera.cameraPivotTransform.transform.rotation = Quaternion.Slerp(camera.cameraPivotTransform.rotation, targetRotation, 0.25f);

        ////Save our rotation values, so when we unlock it doesn't snap too far away
        camera.leftAndRightLookAngle = camera.transform.eulerAngles.y;
        camera.upAndDownLookAngle = camera.transform.eulerAngles.x;
    }
}
public static class TeleportData
{
    public static int SceneID;
    public static string SceneIdString;
    public static Vector3 Destination;
    public static PlayerManager playerManager;
    public static bool enableAfterLoad = true;
    public static float yRotation = 0;
}