using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Fridge
{
    public class FridgeController : Interactable
    {
        // Start is called before the first frame update
        private bool initialized;

        public bool topDoorOpen;
        private bool topDoorOpenTemp;
        public bool bottomDoorOpen;
        private bool bottomDoorOpenTemp;

        private FridgeDoorController[] doorScripts = new FridgeDoorController[] {null,null};

        private LerpScript lightLerp;
        private LerpScript lightLerp2;
        private Light topLight;
        private Light bottomLight;

        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            LightHandler(topLight,"Top",lightLerp);
            LightHandler(bottomLight,"Bottom",lightLerp2);
        }

        void Initialize()
        {
            doorScripts[0] = transform.GetChild(1).GetChild(0).GetComponent<FridgeDoorController>();
            doorScripts[1] = transform.GetChild(1).GetChild(1).GetComponent<FridgeDoorController>();
            topDoorOpen = doorScripts[0].open;
            bottomDoorOpen = doorScripts[1].open;
            lightLerp = this.AddComponent<LerpScript>();
            lightLerp.typeOfLerp = LerpScript.LerpType.Float;
            lightLerp.lerpSpeed = 2;
            lightLerp2 = this.AddComponent<LerpScript>();
            lightLerp2.typeOfLerp = LerpScript.LerpType.Float;
            lightLerp2.lerpSpeed = 2;
            topLight = transform.GetChild(3).GetComponent<Light>();
            bottomLight = transform.GetChild(2).GetComponent<Light>();

            initialized = true;
        }

        void LightHandler(Light light, string topOrBottom, LerpScript _lerpScript)
        {
            switch (topOrBottom)
            {
                case "Top":
                {
                    if (topDoorOpenTemp != topDoorOpen)
                    {
                        if (topDoorOpen)
                        {
                            _lerpScript.floatTarget = 1;
                            topDoorOpenTemp = true;
                        }
                        else
                        {
                            _lerpScript.floatTarget = 0;
                            topDoorOpenTemp = false;
                        }
                    }
                    break;
                }
                case "Bottom":
                {
                    if (bottomDoorOpenTemp != bottomDoorOpen)
                    {
                        if (bottomDoorOpen)
                        {
                            if (!light.enabled)
                            {
                                light.enabled = true;
                            }//enables light if off
                            _lerpScript.floatTarget = 1;
                            bottomDoorOpenTemp = true;
                        }
                        else
                        {
                            _lerpScript.floatTarget = 0;
                            bottomDoorOpenTemp = false;
                        }
                    }
                    break;
                }
            }
            if (_lerpScript.floatVal != _lerpScript.floatTarget)//updates the actual value based on the lerp script
            {
                light.intensity = 0.37f * _lerpScript.floatVal;
                if (light.intensity < 0.01f)
                {
                    light.intensity = 0;
                }
            }
        }
    }
}
