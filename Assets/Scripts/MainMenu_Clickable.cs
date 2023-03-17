using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu_Clickable : MonoBehaviour
{
    public GameObject MainCam;
    public int valueToChangeTo;
    public bool isStartButton;

    private Vector3 position;
    private Vector3 rotation;

    private Vector3 defaultPosition = new Vector3(-1.16f, 1.61f, -0.6f);
    private Vector3 leftPosition = new Vector3(-2.5f, 1.65f, 1.63f);
    private Vector3 rightPosition = new Vector3(.34f, 1.65f, 1.65f);
    private Vector3 centerPosition = new Vector3(-1.16f, 1.61f, 1.67f);

    private Vector3 loadPosition = new Vector3(-1.4104867f, 3.72687531f, 5.35491753f);

    private Vector3 defaultRotation = Vector3.zero;
    private Vector3 leftRotation = new Vector3(0, -90, 0);
    private Vector3 rightRotation = new Vector3(0, 90, 0);
    private Vector3 centerRotation = new Vector3(15, 0, 0);

    private Vector3[] cameraPositions = new Vector3[4];
    private Vector3[] cameraRotations = new Vector3[4];


    //look into world canvas
    // Start is called before the first frame update
    void Start()
    {
        AddValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LeanTween.move(MainCam, defaultPosition, 1.5f).setEaseInOutExpo();
            LeanTween.rotate(MainCam, defaultRotation, 1.5f).setEaseInOutExpo();
        }
    }

    public void AddValues()
    {
        cameraPositions[0] = defaultPosition;
        cameraPositions[1] = leftPosition;
        cameraPositions[2] = rightPosition;
        cameraPositions[3] = centerPosition;

        cameraRotations[0] = defaultRotation;
        cameraRotations[1] = leftRotation;
        cameraRotations[2] = rightRotation;
        cameraRotations[3] = centerRotation;
    }

    public void OnMouseDown()
    {
        if(!isStartButton)
        {
            position = cameraPositions[valueToChangeTo];
            rotation = cameraRotations[valueToChangeTo];
            LeanTween.move(MainCam, position, 1.5f).setEaseInOutExpo();
            LeanTween.rotate(MainCam, rotation, 1.5f).setEaseInOutExpo();
        }
        
        if((MainCam.transform.position.z - centerPosition.z) < 0.1 && isStartButton)
        {
            MainCam.transform.position = loadPosition;
            StartCoroutine(Load());
            
        }
    }

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Granny's House");
    }
}
 