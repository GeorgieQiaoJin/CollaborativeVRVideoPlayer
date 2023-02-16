using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//
namespace Networking.Pun2
{
    public class SetParent : MonoBehaviourPun
    {

        public GameObject image_parent_player1;
        public GameObject image_parent_player2;
        public GameObject image_parent_player3;
        public GameObject image_parent_player4;
        public GameObject image_parent_player5;

        public ScreenShotStore screenShotStore;

        //no start function

        public void SetParentRPC(int n)

        {

            GetComponent<PhotonView>().RPC("RPC_SetParent", RpcTarget.AllBuffered, n);
        }

        [PunRPC]
        void RPC_SetParent(int id)//n is player ID
        {
            screenShotStore = GameObject.Find("ScreenShotStore").GetComponent<ScreenShotStore>();

            //Vector3 startPosition = new Vector3(-0.4f, 0.4f, 0);//based on the parent's position

            //float i = 0f;
            //float j = 0f;

            //int id = screenShotStore.screenshot_store[n].playerID;

            switch (id)
            {
                case 1:
                    image_parent_player1 = GameObject.Find("image_parent_player1");
                    this.transform.parent = image_parent_player1.transform;
                    //this.transform.localPosition = new Vector3(startPosition.x + i, startPosition.y + j, 0);
                    break;
                case 2:
                    image_parent_player2 = GameObject.Find("image_parent_player2");
                    this.transform.parent = image_parent_player2.transform;
                    //this.transform.localPosition = new Vector3(startPosition.x + i, startPosition.y + j, 0);

                    break;
                case 3:
                    image_parent_player3 = GameObject.Find("image_parent_player3");
                    this.transform.parent = image_parent_player3.transform;
                    //this.transform.localPosition = new Vector3(startPosition.x + i, startPosition.y + j, 0);
                    break;
                case 4:
                    image_parent_player4 = GameObject.Find("image_parent_player4");
                    this.transform.parent = image_parent_player4.transform;
                    //this.transform.localPosition = new Vector3(startPosition.x + i, startPosition.y + j, 0);
                    break;
                case 5:
                    image_parent_player5 = GameObject.Find("image_parent_player5");
                    this.transform.parent = image_parent_player5.transform;
                    //this.transform.localPosition = new Vector3(startPosition.x + i, startPosition.y + j, 0);
                    break;
                default:
                    image_parent_player5 = GameObject.Find("image_parent_player5");
                    this.transform.parent = image_parent_player5.transform;
                    //this.transform.localPosition = new Vector3(startPosition.x + i, startPosition.y + j, 0);
                    break;
            }

           
        }
    }

}