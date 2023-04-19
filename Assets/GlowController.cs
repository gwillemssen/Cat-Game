using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowController : MonoBehaviour
{
    public GameObject trackedObj;  // Start is called before the first frame update
    public MeshRenderer meshRenderer;
    public float maxRange;
    public float minRange;
    [SerializeField]private float distance;
    public float actualDistance;

    // Update is called once per frame
    void Update()
    {
        distance = Mathf.Lerp(1,0,Mathf.Clamp01((Vector3.Distance(trackedObj.transform.position, transform.position)-minRange) / (maxRange-minRange)));
        actualDistance = Vector3.Distance(trackedObj.transform.position, transform.position);
        meshRenderer.material.SetFloat("_Closeness", distance);
    }
}
