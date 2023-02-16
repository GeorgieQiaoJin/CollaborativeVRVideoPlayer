using Photon.Pun;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//
//Creation example tool to instantiate a cube in the network using PhotonNetwork.Instantiate
//The cube ownership is set to actor number when created, and to its color using SetColor RPC
//
namespace Networking.Pun2
{
    public class CubeTool : MonoBehaviourPun
    {
        [SerializeField] enum Hand { Right, Left };
        [SerializeField] Hand hand;
        bool building;
        float t;

        void Update()
        {
            if (photonView.IsMine)
            {
                if (hand == Hand.Left)
                {
                    t = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
                }
                else if (hand == Hand.Right)
                {
                    t = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);
                }

                if (t > 0.5f && !building)
                {
                    building = true;
                }
                if (building && t < 0.5f)
                {
                    building = false;
                    ReleaseCube();
                }


                bool c = false;
                if (hand == Hand.Left)
                {
                    c = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch);
                }
                else if (hand == Hand.Right)
                {
                    c = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch);
                }

                if (c)
                {
                    DeleteCubes();
                }

            }
        }

        void ReleaseCube()
        {
            GameObject obj = PhotonNetwork.Instantiate("GenericCube", transform.position, transform.rotation, 0);
            obj.GetComponent<SetColor>().SetColorRPC(PhotonNetwork.LocalPlayer.ActorNumber, false);
        }

        void DeleteCubes()
        {
            Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(0.025f, 0.025f, 0.025f), transform.rotation);
            foreach (var hit in hitColliders)
            {
                PhotonView pv = hit.GetComponent<PhotonView>();
                if (hit.CompareTag("PlayerItem") && pv && pv.IsMine && pv.AmOwner)
                    PhotonNetwork.Destroy(hit.gameObject);
            }
        }
    }
}
