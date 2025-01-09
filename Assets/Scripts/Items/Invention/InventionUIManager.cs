using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventionUIManager : MonoBehaviour
{
    GameObject photoFrame;
    IEnumerator LoadScreenshot(IdeaType type)
    {
        string filePath = Application.dataPath + "/screenshot.png";
        if (System.IO.File.Exists(filePath))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            photoFrame.GetComponent<Renderer>().material.mainTexture = texture;
        }
        yield return null;
    }
    void TakeScreenshot()
    {
        //ScreenCapture.CaptureScreenshot("SomeLevel.png");
        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        Rect rect = new Rect(0, 0, width, height);
        screenshot.ReadPixels(rect, 0, 0);
        screenshot.Apply();
        byte[] bytes = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/screenshot.png", bytes);
        Debug.Log("Screenshot saved at: " + Application.dataPath + "/screenshot.png");
    }
}
