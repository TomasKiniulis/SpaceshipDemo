using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.Graphics;

namespace Tests
{
    public class TestScript
    {
        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator TestScriptSimplePasses()
        {
            Texture2D refImage1;
            string imagePath = "Assets/ReferenceImages/Linear/WindowsEditor/Direct3D11/None";

            SceneManager.LoadScene("Assets/Scenes/Spaceship/Spaceship.unity", LoadSceneMode.Additive);
            SceneManager.LoadScene("Assets/Scenes/Spaceship/Spaceship_FX.unity", LoadSceneMode.Additive);
            SceneManager.LoadScene("Assets/Scenes/Spaceship/Spaceship_GR.unity", LoadSceneMode.Additive);
            SceneManager.LoadScene("Assets/Scenes/Spaceship/Spaceship_LD.unity", LoadSceneMode.Additive);
            SceneManager.LoadScene("Assets/Scenes/Spaceship/Spaceship_LI.unity", LoadSceneMode.Additive);
            SceneManager.LoadScene("Assets/Scenes/Spaceship/Spaceship_SD.unity", LoadSceneMode.Additive);

            SceneManager.LoadScene("Assets/Scenes/Spaceship/Spaceship_Wrecked_FX.unity", LoadSceneMode.Additive);
            SceneManager.LoadScene("Assets/Scenes/Spaceship/Spaceship_Wrecked_GR.unity", LoadSceneMode.Additive);
            SceneManager.LoadScene("Assets/Scenes/Spaceship/Spaceship_Wrecked_LD.unity", LoadSceneMode.Additive);
            SceneManager.LoadScene("Assets/Scenes/Spaceship/Spaceship_Wrecked_LI.unity", LoadSceneMode.Additive);
            SceneManager.LoadScene("Assets/Scenes/Spaceship/Spaceship_Wrecked_SD.unity", LoadSceneMode.Additive);
            // Use the Assert class to test conditions

            // Arbitrary wait for 5 frames for the scene to load, and other stuff to happen (like Realtime GI to appear ...)
            for (int i = 0; i < 20; ++i)
            {
                yield return null;
            }
            GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
            Camera camera = cameraObj.GetComponent<Camera>();

            HDRP_TestSettings settings = cameraObj.AddComponent<HDRP_TestSettings>();

            ImageAssert.AreEqual(null, camera, settings?.ImageComparisonSettings);
        }
    }
}
