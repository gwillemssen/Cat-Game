using Unity.VisualScripting;
using UnityEngine;

namespace Fridge
{
    public class FridgeDoorController : Interactable
    {
        public int ID;
        
        private LerpScript doorLerp;

        private GameObject doorHinge;

        public bool open;

        private Vector3 doorValue;

        private FridgeController parentScript;
        // Start is called before the first frame update
        void Start()
        {
            parentScript = transform.parent.parent.GetComponent<FridgeController>();
            
            doorLerp = this.AddComponent<LerpScript>();
            doorLerp.typeOfLerp = LerpScript.LerpType.Vector3;
            doorLerp.lerpSpeed = 4;
        }

        // Update is called once per frame
        void Update()
        {
            DoorHandler();
        }
    
        public override void Interact(FirstPersonController controller)
        {
            FridgeActivation();
        }

        private void DoorHandler()
        {
            if (ID == 0)
            {
                open = parentScript.bottomDoorOpen;
            }
            else
            {
                open = parentScript.topDoorOpen;
            }
            
            if (open)
            {
                if (doorLerp.vecTarget != new Vector3(0, 0, -130))
                {
                    doorLerp.vecTarget = new Vector3(0, 0, -130);
                }
            }
            else
            {
                if (doorLerp.vecTarget != Vector3.zero)
                {
                    doorLerp.vecTarget = new Vector3(0, 0, 0);
                }
            }
            transform.localRotation = Quaternion.Euler(doorLerp.vecVal);
        }

        private void FridgeActivation()
        {
            if (ID == 0)
            {
                parentScript.bottomDoorOpen = !parentScript.bottomDoorOpen;
            }
            else
            {
                parentScript.topDoorOpen = !parentScript.topDoorOpen;
            }
        }
    
    }
}
