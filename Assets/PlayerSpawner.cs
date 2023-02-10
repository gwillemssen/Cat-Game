using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        if (GameObject.Find("Player") != null)
        {
            Debug.LogError("PlayerSpawner : a Player already exists");
            Destroy(this.gameObject);
            return;
        }
        
        GameObject player = Instantiate(Resources.Load<GameObject>("Player"), this.transform.position, this.transform.rotation);
    }
}
