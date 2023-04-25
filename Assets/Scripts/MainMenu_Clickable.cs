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

    private Vector3 position;
    private Vector3 rotation;

    private static Vector3 defaultPosition = new Vector3(-1.16f, 1.61f, -0.6f);
    private static Vector3 leftPosition = new Vector3(-2.5f, 1.65f, 1.63f);
    private static Vector3 rightPosition = new Vector3(.34f, 1.65f, 1.65f);
    private static Vector3 centerPosition = new Vector3(-1.16f, 1.61f, 1.67f);

    private static Vector3 defaultRotation = Vector3.zero;
    private static Vector3 leftRotation = new Vector3(0, -90, 0);
    private static Vector3 rightRotation = new Vector3(0, 90, 0);
    private static Vector3 centerRotation = new Vector3(15, 0, 0);

    private static GameObject leftArrows;
    private static GameObject rightArrows;
    private static GameObject centerArrowLeft;
    private static GameObject centerArrowRight;
    private static bool setup;

    private static Vector3[] cameraPositions = new Vector3[4];
    private static Vector3[] cameraRotations = new Vector3[4];

    private bool hovering = false;
    private float hoveringTime;
    private Color emissionColor;
    private Color emissionColorStart;
    private Color emissionColorGlow = Color.white * 4f;
    private Material material;

    //look into world canvas
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        emissionColorStart = material.GetColor("_EmissionColor");
        AddValues();

        //this is not the way I would have done this, but im just building on the existing system instead of refactoring it because im in a time crunch
        //bad.
        if(!setup)
        {
            if (leftArrows == null)
            { leftArrows = GameObject.Find("LeftButton_Controls"); }
            if (rightArrows == null)
            { rightArrows = GameObject.Find("RightButton_Controls"); }
            if (centerArrowLeft == null)
            { centerArrowLeft = GameObject.Find("CenterButtonLeft_Controls"); }
            if (centerArrowRight == null)
            { centerArrowRight = GameObject.Find("CenterButtonRight_Controls"); }
            SetArrowsEnabled(false);
            setup = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LeanTween.move(MainCam, defaultPosition, 1.5f).setEaseInOutExpo();
            LeanTween.rotate(MainCam, defaultRotation, 1.5f).setEaseInOutExpo();
        }

        if (hovering)
        { hoveringTime += Time.deltaTime * 8f; }
        else
        { hoveringTime -= Time.deltaTime * 4f; }

        hoveringTime = Mathf.Clamp01(hoveringTime);
        if (material != null)
        { material.SetColor("_EmissionColor", Color.Lerp(emissionColorStart, emissionColorGlow, hoveringTime)); }

        hovering = false;
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
        position = cameraPositions[valueToChangeTo];
        rotation = cameraRotations[valueToChangeTo];
        float duration = 1.5f;
        LeanTween.cancel(MainCam);
        LeanTween.move(MainCam, position, duration).setEaseOutQuart();
        LeanTween.rotate(MainCam, rotation, duration).setEaseOutQuart();
        StartCoroutine(WaitAndEnableArrows(duration / 2f));
    }

    IEnumerator WaitAndEnableArrows(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetArrowsEnabled(true);
    }

    private void SetArrowsEnabled(bool enabled)
    {
        print(enabled);
        leftArrows.SetActive(enabled);
        rightArrows.SetActive(enabled);
        centerArrowLeft.SetActive(enabled);
        centerArrowRight.SetActive(enabled);
    }

    public void OnMouseOver()
    { hovering = true; }

}
 