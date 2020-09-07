using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.IO;

public class HDRP_GraphicTestRunner
{

    [PrebuildSetup("SetupGraphicsTestCases")]
    [UseGraphicsTestCases]
    public IEnumerator Run(GraphicsTestCase testCase)
    {
        SceneManager.LoadScene(testCase.ScenePath);
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

        // Arbitrary wait for 5 frames for the scene to load, and other stuff to happen (like Realtime GI to appear ...)
        for (int i=0 ; i<5; ++i)
            yield return null;

        // Load the test settings
        GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        HDRP_TestSettings settings = cameraObj.GetComponent<HDRP_TestSettings>();

        var camera = cameraObj.GetComponent<Camera>();

        //Define HDRP_TestSettings
        //settings.ImageComparisonSettings.PerPixelCorrectnessThreshold = 0.001f;
        //settings.ImageComparisonSettings.AverageCorrectnessThreshold = 0.001f;
        //settings.waitFrames = 30;
        //settings.captureFramerate = 30;

        if (camera == null) camera = GameObject.FindObjectOfType<Camera>();
        if (camera == null)
        {
            Assert.Fail("Missing camera for graphic tests.");
        }

        Time.captureFramerate = settings.captureFramerate;

        if (settings.doBeforeTest != null)
        {
            settings.doBeforeTest.Invoke();

            // Wait again one frame, to be sure.
            yield return null;
        }

        // Reset temporal effects on hdCamera
        HDCamera.GetOrCreate(camera).Reset();

        for (int i=0 ; i<settings.waitFrames ; ++i)
            yield return null;

        var settingsSG = (GameObject.FindObjectOfType<HDRP_TestSettings>() as HDRP_ShaderGraph_TestSettings);
        if (settingsSG == null || !settingsSG.compareSGtoBI)
        {
            // Standard Test
            ImageAssert.AreEqual(testCase.ReferenceImage, camera, settings?.ImageComparisonSettings);

            if (settings.checkMemoryAllocation)
            {
                // Does it allocate memory when it renders what's on camera?
                bool allocatesMemory = false;
                try
                {
                    // GC alloc from Camera.CustomRender (case 1206364)
                    int gcAllocThreshold = 2;

                    ImageAssert.AllocatesMemory(camera, settings?.ImageComparisonSettings, gcAllocThreshold);
                }
                catch (AssertionException)
                {
                    allocatesMemory = true;
                }
                if (allocatesMemory)
                    Assert.Fail("Allocated memory when rendering what is on camera");
            }
        }
        else
        {
            if (settingsSG.sgObjs == null)
            {
                Assert.Fail("Missing Shader Graph objects in test scene.");
            }
            if (settingsSG.biObjs == null)
            {
                Assert.Fail("Missing comparison objects in test scene.");
            }

            settingsSG.sgObjs.SetActive(true);
            settingsSG.biObjs.SetActive(false);
            yield return null; // Wait a frame
            yield return null;
            bool sgFail = false;
            bool biFail = false;

            // First test: Shader Graph
            try
            {
                ImageAssert.AreEqual(testCase.ReferenceImage, camera, (settings != null)?settings.ImageComparisonSettings:null);
            }
            catch (AssertionException)
            {
                sgFail = true;
            }

            settingsSG.sgObjs.SetActive(false);
            settingsSG.biObjs.SetActive(true);
            settingsSG.biObjs.transform.position = settingsSG.sgObjs.transform.position; // Move to the same location.
            yield return null; // Wait a frame
            yield return null;

            // Second test: HDRP/Lit Materials
            try
            {
                ImageAssert.AreEqual(testCase.ReferenceImage, camera, (settings != null)?settings.ImageComparisonSettings:null);
            }
            catch (AssertionException)
            {
                biFail = true;
            }

            // Informs which ImageAssert failed, if any.
            if (sgFail && biFail) Assert.Fail("Both Shader Graph and Non-Shader Graph Objects failed to match the reference image");
            else if (sgFail) Assert.Fail("Shader Graph Objects failed.");
            else if (biFail) Assert.Fail("Non-Shader Graph Objects failed to match Shader Graph objects.");
        }
    }

#if UNITY_EDITOR

    [TearDown]
    public void DumpImagesInEditor()
    {
        UnityEditor.TestTools.Graphics.ResultsUtility.ExtractImagesFromTestProperties(TestContext.CurrentContext.Test);
    }
#endif

}
