using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//
namespace Networking.Pun2
{
    public class ChangeTexture : MonoBehaviourPun
    {
        public Text DebugLog;
        public GameObject notepanel;


        public ScreenShotStore screenShotStore;

        //no start function

        public void ChangeTextureRPC(int n, int playerID)

        {
            
            GetComponent<PhotonView>().RPC("RPC_ChangeTexture", RpcTarget.AllBuffered, n, playerID);
        }

        [PunRPC]
        void RPC_ChangeTexture(int n, int playerID)
        {
            DebugLog = GameObject.Find("/Canvas/DebugLog").GetComponent<Text>();

           //notepanel = .Find("/Canvas/DebugLog").GetComponent<Text>();

            DebugLog.text = "ChangeTextureRPC";
            screenShotStore = GameObject.Find("ScreenShotStore").GetComponent<ScreenShotStore>();

            if (GetComponentInChildren<RawImage>() != null)
            {
                //DebugLog.text = "screenshot_store.Count: " + screenShotStore.screenshot_store.Count.ToString();
                //GetComponentInChildren<RawImage>().texture = screenShotStore.screenshot_store[n].image;
                //if (screenShotStore.screenshot_store[n].note != "")
                //{
                //    GetComponentInChildren<Image>().enabled = true;
                //    GetComponentInChildren<Text>().text = screenShotStore.screenshot_store[n].note;
                //}

                DebugLog.text = "screenshot_store.Count: " + screenShotStore.screenshot_store_p[playerID].Count.ToString();
                GetComponentInChildren<RawImage>().texture = screenShotStore.screenshot_store_p[playerID][n].image;
                if (screenShotStore.screenshot_store_p[playerID][n].note != "")
                {
                    GetComponentInChildren<Text>().text = screenShotStore.screenshot_store_p[playerID][n].note;
                }

            }
            else
            {
                DebugLog.text = "RPC_ChangeTexture-Fail";
            }

                

            //GetComponent<RawImage>().texture = ss;
        }
    }

}