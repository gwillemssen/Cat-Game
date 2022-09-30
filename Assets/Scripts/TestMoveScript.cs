using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestMoveScript : MonoBehaviour
{
    public bool gottem;

    public GameObject wowcube;

    private Vector3 destination;

    private LerpScript2 mover;
    // Start is called before the first frame update
    void Start()
    {
        mover = this.AddComponent<LerpScript2>();
        
        // mover = new LerpScript2(this.transform.position, destination);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // if (gottem)
        // {
        //     destination = wowcube.transform.position;
        // }
        // else
        // {
        //     destination = this.transform.position;
        // }
        //
        // transform.position = mover.vectorValues[0];
    }
}
