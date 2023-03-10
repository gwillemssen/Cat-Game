using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public GameObject MainCam;
    private Vector3 centerPosition = new Vector3(-1.16f, 1.61f, 1.67f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        if(Vector3.SqrMagnitude(centerPosition - MainCam.transform.position) < .01)
        {
            
            SceneManager.LoadScene("Granny's House");
        }

        
    }
}
