using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//
//Sets the color of the first MeshRenderer/SkinnedMeshRenderer found with GetComponentInChildren
//
namespace Networking.Pun2
{
    public class SetColor : MonoBehaviourPun
    {
        public Color playerColor;
        

        public void SetColorRPC(int n, bool Nsync)
        {
            GetComponent<PhotonView>().RPC("RPC_SetColor", RpcTarget.AllBuffered, n, Nsync);
        }

        [PunRPC]
        void RPC_SetColor(int n, bool Nsync)
        {

            Color32 red = new Color32(199, 59, 11, 255);
            Color32 blue = new Color32(86, 180, 233,255);
            Color32 green = new Color32(37, 190, 103, 255);
            Color32 orange = new Color32(230, 159, 0, 255);
            Color32 yellow = new Color32(239, 199, 132, 1);
            
            switch (n)
            {
                case 1:
                    playerColor = orange;// Color.red;
                    break;
                case 2:
                    playerColor = blue;//Color.cyan;
                    break;
                case 3:
                    playerColor = green;// Color.green;
                    break;
                case 4:
                    playerColor = Color.yellow;
                    break;
                case 5:
                    playerColor = Color.magenta;
                    break;
                default:
                    playerColor = Color.black;
                    break;
            }
            
            playerColor = Color.Lerp(Color.white, playerColor, 0.5f);
            if(GetComponentInChildren<MeshRenderer>() != null)
                GetComponentInChildren<MeshRenderer>().material.color = playerColor;
            else if (GetComponentInChildren<SkinnedMeshRenderer>() != null)
                GetComponentInChildren<SkinnedMeshRenderer>().material.color = playerColor;

            //if (Nsync && !photonView.IsMine)
            if (Nsync)
             {
                //if (GetComponentInChildren<MeshRenderer>() != null)
                //    GetComponentInChildren<MeshRenderer>().enabled = false;
                //else if (GetComponentInChildren<SkinnedMeshRenderer>() != null)
                //    GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                MeshRenderer[] meshRenderers;
                meshRenderers = GetComponentsInChildren<MeshRenderer>();
                SkinnedMeshRenderer[] skinnedMeshRenderers;
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                if (meshRenderers.Length > 0)
                {
                    foreach (MeshRenderer meshRenderer in meshRenderers)
                        meshRenderer.enabled = false;
                }
                if (skinnedMeshRenderers.Length > 0)
                {
                    foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
                        skinnedMeshRenderer.enabled = false;
                }

            }
        }
    }
}
