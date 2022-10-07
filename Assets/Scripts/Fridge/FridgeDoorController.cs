using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FridgeDoorController : MonoBehaviour
{
    public int ID;
    private LerpScript doorLerp;
    // Start is called before the first frame update
    void Start()
    {
        doorLerp = this.AddComponent<LerpScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
