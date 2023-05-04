using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

public class Toilet : GenericHinge
{
    [SerializeField] private float delay;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Interact(FirstPersonController controller)
    {
        //StartCoroutine(OpenThenClose(delay));
        OpenHinge(Hinge, DefaultPos, TransformPos, OpeningDuration, ClosingDuration);
    }

    public IEnumerator OpenThenClose(float Delay)
    {
        print("triggered!");
        OpenHinge(Hinge, DefaultPos, TransformPos, OpeningDuration, ClosingDuration);
        yield return new WaitForSeconds(Delay);
        OpenHinge(Hinge, DefaultPos, TransformPos, OpeningDuration, ClosingDuration);
        
    }
}
