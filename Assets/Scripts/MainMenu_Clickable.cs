using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Clickable : MonoBehaviour
{

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
            Camera.main.transform.position = defaultPosition;
            Camera.main.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void OnMouseDown()
    {
        Camera.main.transform.position = postion;
        Camera.main.transform.localRotation = Quaternion.Euler(rotation);
    }
}
