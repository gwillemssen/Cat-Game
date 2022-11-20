using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toilet_script : MonoBehaviour
{

    public GameObject Pivot;
    public Vector3 rotated = new Vector3(26, 0, 0);
    private Vector3 original;
    private bool flushing;
    // Start is called before the first frame update
    void Start()
    {
        original = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (flushing)
        {
            //nothing should happen / cannot flush while flushing
        }
        else
        {
            flushing = true;
            LeanTween.rotate(Pivot, rotated, 1);
            LeanTween.rotate(Pivot, original, 1);
            //Play SFX
            flushing = false;
           
        }
    }
}
