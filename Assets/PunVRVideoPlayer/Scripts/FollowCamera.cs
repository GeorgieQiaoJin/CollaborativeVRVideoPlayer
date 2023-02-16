using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Networking.Pun2
{
    public class FollowCamera : MonoBehaviourPun
    {
        public GameObject MainCam;
        // Start is called before the first frame update
        void Start()
        {
            MainCam = GameObject.Find("/OVRNetworkCameraRig/TrackingSpace/CenterEyeAnchor");
        }

        void LateUpdate()
        {
            if (photonView.IsMine)
            {
                transform.position = MainCam.transform.position;
                transform.rotation = MainCam.transform.rotation;
            }
        }
    }
}
