using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Clickable : MonoBehaviour
{
    public GameObject MainCam;
    public Vector3 postion;
    public Vector3 rotation; 
    public Vector3 defaultPosition;
    //look into world canvas
    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = Camera.main.transform.position;
        
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

        LeanTween.move(MainCam, postion, 1.5f).setEaseInOutExpo();
        LeanTween.rotate(MainCam, rotation,1.5f).setEaseInOutExpo();
    }
}
