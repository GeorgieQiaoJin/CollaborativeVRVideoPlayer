using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking.Pun2;
using Photon.Pun;
using static Networking.Pun2.PunOVRGrabbable;

public class Scalable : MonoBehaviour
{
    float swapTimer = 0;
    float scaleStep = 0.02f;
    public PunOVRGrabbable grabbableCube;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbableCube.getGrabStatus() == true)
        {
            if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y > 0.6)
            {
                swapTimer += Time.deltaTime;
                if (swapTimer > 0.2f)
                {
                    swapTimer = 0;
                    if(this.transform.localScale.x >= 0.2f)
                    {
                        //do nothing
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(transform.localScale.x + scaleStep, transform.localScale.y + scaleStep, transform.localScale.z + scaleStep);
                    }
                }
            }
            else if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y < -0.6)
            {
                swapTimer += Time.deltaTime;
                if (swapTimer > 0.2f)
                {
                    swapTimer = 0;
                    if(this.transform.localScale.x <= 0.002f)
                    {
                        //do nothing;
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(transform.localScale.x - scaleStep, transform.localScale.y - scaleStep, transform.localScale.z - scaleStep);

                    }
                }
            }
        }
    }
}
