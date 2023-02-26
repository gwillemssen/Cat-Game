using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Break : MonoBehaviour
{
    public bool breakOnImpact;
    public float velocityToBreak = 10;
    public GameObject prefab;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Shatter()
    {
        GetComponent<Collider>().enabled = false;
        var velocity = rb.velocity;
        GameObject brokevase = Instantiate(prefab, transform.position, transform.rotation);
        brokevase.transform.localScale = transform.localScale;
        foreach (Transform child in brokevase.transform)
        {
            child.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);
        }
        Destroy(this.GameObject());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (breakOnImpact && rb.velocity.magnitude >= velocityToBreak)
            Shatter();
    }
}
