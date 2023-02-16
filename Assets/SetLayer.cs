using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class SetLayer : MonoBehaviour
{
    public Text debuglog;

    public void SetLayerRPC(int id)
    {
        GetComponent<PhotonView>().RPC("RPC_SetLayer", RpcTarget.All, id);
    }

    [PunRPC]
    void RPC_SetLayer(int PlayerID)
    {
        
        //debuglog = GameObject.Find("/Canvas/DebugLog").GetComponent<Text>();

        switch (PlayerID)
        {
            case 1:
                this.gameObject.layer = 8;
                //debuglog.text = this.gameObject.layer.ToString();
                foreach (Transform tran in this.gameObject.GetComponentInChildren<Transform>())
                {
                    tran.gameObject.layer = 8;
                }
                
                break;
            case 2:
                this.gameObject.layer = 9;
                //debuglog.text = this.gameObject.layer.ToString();
                //brushStrokeObject.layer = LayerMask.NameToLayer("Annotation2");
                foreach (Transform tran in this.gameObject.GetComponentInChildren<Transform>())
                {
                    debuglog.text = tran.ToString();
                    tran.gameObject.layer = 9;
                }
                break;
            case 3:
                this.gameObject.layer = 10;
                //debuglog.text = this.gameObject.layer.ToString();
                //brushStrokeObject.layer = LayerMask.NameToLayer("Annotation3");
                foreach (Transform tran in this.gameObject.GetComponentInChildren<Transform>())
                {
                    tran.gameObject.layer = 10;
                }
                break;
            default:
                this.gameObject.layer = 10;
                
                foreach (Transform tran in this.gameObject.GetComponentInChildren<Transform>())
                {
                    tran.gameObject.layer = 10;
                }
                break;

        }
    }
}
