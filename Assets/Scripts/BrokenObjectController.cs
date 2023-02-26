using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObjectController : MonoBehaviour
{
    public float fragmentLifetime = 5;
    private List<Rigidbody> childRbs = new List<Rigidbody>();
    private float timer;
    private bool afterStart;
    
    void Start()
    {
        foreach (Transform child in transform)
        {
            childRbs.Add(child.GetComponent<Rigidbody>());
        }
        afterStart = true;
    }
    
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= 1 && childRbs.Count == 0)
        {
            Destroy(gameObject);
            print($"Cleaned up {gameObject.name}");
        }
        
        foreach (var rb in childRbs)
        {
            if (afterStart && timer >=1)
            {
                if(rb.velocity == Vector3.zero)
                    rb.isKinematic = true;
            }

            if (timer >= fragmentLifetime && !LeanTween.isTweening(rb.gameObject))
            {
                LeanTween.scale(rb.gameObject, Vector3.zero, 1).setEaseOutExpo().setOnComplete(
                    delegate()
                    {
                        LeanTween.cancel(rb.gameObject);
                        childRbs.Remove(rb);
                        Destroy(rb.gameObject);
                    });
            }
        }
    }
}
