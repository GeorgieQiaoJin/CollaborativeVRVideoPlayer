using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Networking.Pun2
{
    public class TeleportTool : LoggableTool
    {
        [SerializeField] private OVRInput.Controller hand;

        public LineRenderer laserPointer;
        public Material teleportTargetMaterial;
        private Material lineRendererMaterial;
        private Transform rigPos;
        private GameObject teleportMarker;

        void Awake()
        {
            lineRendererMaterial = laserPointer.material;
            rigPos = OculusPlayer.instance.transform;
            teleportMarker = PhotonNetwork.Instantiate("TeleportMarker", new Vector3(0, -2, 0), Quaternion.identity);
        }

        private void OnDisable()
        {
            teleportMarker.transform.position = new Vector3(0, -2, 0);
            photonView.RPC("ToggleTeleportMarker", RpcTarget.All, false);
        }

        // Update is called once per frame
        public override void Update()
        {
            if (!photonView.IsMine) return;
            base.Update();

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                laserPointer.SetPosition(1, new Vector3(0, 0, hit.distance));
                if (hit.collider.CompareTag("Floor"))
                {

                    laserPointer.material = teleportTargetMaterial;
                    teleportMarker.transform.position = hit.point; //+ Vector3.up;
                    if (!teleportMarker.activeSelf)
                        photonView.RPC("ToggleTeleportMarker", RpcTarget.All, true);
                    if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, hand))
                        Teleport(hit);
                }
                else
                {
                    laserPointer.material = lineRendererMaterial;
                    if (teleportMarker.activeSelf)
                        photonView.RPC("ToggleTeleportMarker", RpcTarget.All, false);
                }
            }
            else
            {
                laserPointer.SetPosition(1, new Vector3(0, 0, 100));
                laserPointer.material = lineRendererMaterial;
            }

        }

        public void Teleport(RaycastHit hit)
        {
            //float y = camPos.localRotation.eulerAngles.y;
            //Quaternion q = Quaternion.Inverse(Quaternion.Euler(0, y, 0));
            //rigPos.SetPositionAndRotation(hit.point + hit.normal * 0.1f, teleportMarker.transform.rotation * q);
            rigPos.SetPositionAndRotation(hit.point + hit.normal * 0.1f, rigPos.rotation);
        }

        [PunRPC]
        public void ToggleTeleportMarker(bool state)
        {
            teleportMarker.SetActive(state);
        }

    }
}
