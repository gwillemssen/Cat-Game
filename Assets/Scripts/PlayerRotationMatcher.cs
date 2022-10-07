using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationMatcher : MonoBehaviour
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = player.transform.rotation;
    }
}
