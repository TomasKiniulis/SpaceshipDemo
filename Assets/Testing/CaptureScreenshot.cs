using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CaptureScreenshot : MonoBehaviour
{
    public void TakeScreenshot()
    {
        string folderPath = Directory.GetCurrentDirectory() + "/Screenshots/";

        if (!System.IO.Directory.Exists(folderPath))
            System.IO.Directory.CreateDirectory(folderPath);

        var screenshotName =
                                "Screenshot_" +
                                System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") +
                                ".png";
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName));
        Debug.Log(folderPath + screenshotName);
    }

}
