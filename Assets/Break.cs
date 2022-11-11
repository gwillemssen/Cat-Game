using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Break : MonoBehaviour
{
    public GameObject prefab;

    private void OnMouseDown()
    {
        Instantiate(prefab);
        Destroy(this.GameObject());
    }
}
