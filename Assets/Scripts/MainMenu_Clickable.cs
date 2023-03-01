using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu_Clickable : MonoBehaviour
{
    public GameObject MainCam;

    private Vector3 position;
    private Vector3 rotation;
    private Vector3 defaultPosition;
    private Vector3 defaultRotation;

    //public enum test
    //{
    //left = 0, right = 1, center = 2
    //}
    public int LRCPosition;
    private int LRC
    public int LRDirection;
    public string objType;
    private Vector3[] cameraPositions = new Vector3[3] 
    {new Vector3(-2.62f, 1.65f, 1.63f), new Vector3(-1.16f, 1.61f, 1.67f),new Vector3(.45f, 1.65f, 1.65f) };
    private Vector3[] cameraRotations = new Vector3[3]
    {new Vector3(0, -90, 0),new Vector3(15, 0, 0),new Vector3(0, 90, 0)};


    //look into world canvas
    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = Camera.main.transform.position;
        defaultRotation = Camera.main.transform.position; 
        

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LeanTween.move(MainCam, defaultPosition, 1.5f).setEaseInOutExpo();
            LeanTween.rotate(MainCam, Vector3.zero, 1.5f).setEaseInOutExpo();
          
        }
    }

    public void OnMouseDown()
    {
        LRDirection
        if (objType == "arrow")
        {
            LRC += LRDirection;
        }
        Debug.Log($"{LRC}");
        position = cameraPositions[LRC];
        rotation = cameraRotations[LRC];
        LeanTween.move(MainCam, position, 1.5f).setEaseInOutExpo();
        LeanTween.rotate(MainCam, rotation, 1.5f).setEaseInOutExpo();

    }
}
 