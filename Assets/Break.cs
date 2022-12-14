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
        Instantiate(prefab,transform.position,transform.rotation);
        prefab.transform.localScale = transform.localScale;
        foreach (Transform child in prefab.transform)
        {
            child.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
        }
        Destroy(this.GameObject());
    }
}
