using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Reference https://www.youtube.com/watch?v=C1my9i_5RDw
public class AggroRadius : MonoBehaviour
{
    private Transform playerPos;
    private float distance;
    public float enemyMovSpeed;
    //Vector3.sqrMagnitude
    SphereCollider SphereCollider;
    public float howClose;
    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        SphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(playerPos.position, transform.position);
        if (distance<=howClose)
        {
            transform.LookAt(playerPos);
            GetComponent<Rigidbody>().AddForce(transform.forward * enemyMovSpeed);

        }
    }
    
}
