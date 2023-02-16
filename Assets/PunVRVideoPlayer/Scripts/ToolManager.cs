using Photon.Pun;
using UnityEngine;
//
//For managing different tools over the network
//
namespace Networking.Pun2
{
    public class ToolManager : MonoBehaviourPun
    {
        [PunRPC]
        public void DisableTool(int n, bool Nsync)
        {
            if (Nsync && !this.photonView.IsMine)
                return ;
            transform.GetChild(n).gameObject.SetActive(false);
        }

        [PunRPC]
        public void EnableTool(int n, bool Nsync)
        {
            if (Nsync && !this.photonView.IsMine)
                return ;
            transform.GetChild(n).gameObject.SetActive(true);
        }
    }
}
