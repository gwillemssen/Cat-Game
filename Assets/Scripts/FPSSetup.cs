using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FPSSetup : MonoBehaviour
{
    private Camera _mainCamera;
    void Start()
    {
        //adds the camera on top of the render stack
        var thisCam = this.GetComponent<Camera>();
        _mainCamera = Camera.main;
        var cameraData = _mainCamera.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Add(thisCam);
        
        ScreenSpaceCheckSetup();
    }
    
    void ScreenSpaceCheckSetup()//checks that the screenspace layer has been added and culls that layer out from the main camera.
    {
        var newLayer = LayerMask.NameToLayer("ScreenCam");
        if (newLayer <= -1)
        {
            Debug.Log("ScreenSpace layer not found! Please create it.");
        }
        else
        {
            {
                _mainCamera.cullingMask &=  ~(1 << LayerMask.NameToLayer("ScreenCam"));
            }
        }
    }
}
