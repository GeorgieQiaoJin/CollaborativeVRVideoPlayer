using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using OVRTouchSample;

public class InputManager : MonoBehaviour
{
    public GameObject menu; // Assign in inspector
    private bool isShowing;

    private bool isDown;

    // Start is called before the first frame update
    void Start()
    {
        isShowing = true;
        isDown = false;
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.8f && (!isDown))
        {
            isDown = true;
            isShowing = !isShowing;
            menu.SetActive(isShowing);
        }

        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) < 0.2f)
            isDown = false;
    }
}
