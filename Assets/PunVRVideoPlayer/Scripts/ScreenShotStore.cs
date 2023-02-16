using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;


namespace Networking.Pun2
{
    // [RequireComponent(typeof(PhotonView))]
    //public class ScreenShotStore : MonoBehaviourPunMonoBehaviourPunCallbacks, MonoBehaviourPun, 
    public class ScreenShotStore : MonoBehaviourPunCallbacks
    {
        public List<ScreenShot> screenshot_store = new List<ScreenShot>();
        public List<List<ScreenShot>> screenshot_store_p = new List<List<ScreenShot>>(); // List of List<int>
        public Texture2D ss_image;
        public int ss_playerID;
        public float ss_progress;

        public Text DebugLogSS;
        public Text DebugLog;

        public Image Display;//prefab
        public GameObject image_parent_player1;
        public GameObject image_parent_player2;
        public GameObject image_parent_player3;
        public GameObject image_parent_player4;
        public GameObject image_parent_player5;

        public List<GameObject> previousCubes;

        PhotonView mypv;
        public GameObject DisplayImage;
        public LogSSnumber LogSSnumber;

        public float i1 = 0f;
        public float j1 = 0f;
        public float i2 = 0f;
        public float j2 = 0f;
        public float i3 = 0f;
        public float j3 = 0f;
        public float i4 = 0f;
        public float j4 = 0f;
        public float i5 = 0f;
        public float j5 = 0f;

        public string GoToScene2;

        public int total_count = 0;

        public void Start()
        {
            screenshot_store_p.Add(new List<ScreenShot>());
            screenshot_store_p.Add(new List<ScreenShot>());
            screenshot_store_p.Add(new List<ScreenShot>());
            screenshot_store_p.Add(new List<ScreenShot>());
            screenshot_store_p.Add(new List<ScreenShot>());
            screenshot_store_p.Add(new List<ScreenShot>());
        }

        public void EnqueueScreenshot(ScreenShot screenShot)
        {

            //screenshot_store.Add(screenShot);
            screenshot_store_p[screenShot.playerID].Add(screenShot);

            total_count += 1;

        }


        //put the screenshot into the canvas
        public void ShowScreenshot()
        {
            
            if (PhotonNetwork.IsMasterClient)
            {
                DebugLog.text = "click into ShowScreenshot()";
                
                previousCubes = new List<GameObject>();
                displayall();
                
                StaticClass.cur_videoname = "";
                photonView.RPC("GoToSceen", RpcTarget.All, GoToScene2);
            }
            //showimage();
            //pv.RPC("RPC_showimage", RpcTarget.All);

        }


        public void showimage()
        {

            int m = 0;

            foreach (List<ScreenShot> ss_store in screenshot_store_p)
            { 
                foreach (ScreenShot ss in ss_store)
                {
                    Debug.Log("m:" + m);
                    previousCubes[m].GetComponentInChildren<RawImage>().texture = ss.image;
                    m = m + 1;
                }
            }
            
            
        }

        public void displayall()
        {
            //DestroyAllCubes();
            //Image display_image;
            //PhotonNetwork.Destroy();

            //display_image = Display;
           

            //int index_rec = 0;

            int num_row = total_count / 5; //6 images per row

            if (total_count % 5 != 0)
            {
                num_row += 1;//total number of row
            }

            Vector3 startPosition = new Vector3(-0.7f, 0.4f, 0);//based on the parent's position

            int l = 0;


            foreach (List<ScreenShot> ss_store in screenshot_store_p)
            {
                if (ss_store.Count == 0) continue;

                l = 0;


                foreach (ScreenShot ss in ss_store)
                {
                    ss_playerID = ss.playerID;
                    ss_progress = ss.progress;


                    DisplayImage = PhotonNetwork.Instantiate("GenericCube", transform.position, transform.rotation, 0);


                    DisplayImage.GetComponent<SetColor>().SetColorRPC(ss.playerID, false);
                    DisplayImage.GetComponent<ChangeTexture>().ChangeTextureRPC(l, ss.playerID);
                    // previousCubes.Add(DisplayImage);
                    DisplayImage.GetComponent<SetParent>().SetParentRPC(ss.playerID);

                    l = l + 1;

                    switch (ss.playerID)
                    {
                        case 1:
                            //DisplayImage.transform.parent = image_parent_player1.transform;
                            DisplayImage.transform.localPosition = new Vector3(startPosition.x + i1, startPosition.y + j1, 0);
                            DisplayImage.transform.localEulerAngles = new Vector3(0, 0, 0);
                            (i1, j1) = updatePosition(i1, j1);
                            break;
                        case 2:
                            // DisplayImage.transform.parent = image_parent_player2.transform;
                            DisplayImage.transform.localPosition = new Vector3(startPosition.x + i2, startPosition.y + j2, 0);
                            DisplayImage.transform.localEulerAngles = new Vector3(0, 0, 0);

                            (i2, j2) = updatePosition(i2, j2);
                            break;
                        case 3:
                            // DisplayImage.transform.parent = image_parent_player3.transform;
                            DisplayImage.transform.localPosition = new Vector3(startPosition.x + i3, startPosition.y + j3, 0);
                            DisplayImage.transform.localEulerAngles = new Vector3(0, 0, 0);

                            (i3, j3) = updatePosition(i3, j3);
                            break;
                        case 4:
                            // DisplayImage.transform.parent = image_parent_player4.transform;
                            DisplayImage.transform.localPosition = new Vector3(startPosition.x + i4, startPosition.y + j4, 0);
                            DisplayImage.transform.localEulerAngles = new Vector3(0, 0, 0);

                            (i4, j4) = updatePosition(i4, j4);
                            break;
                        case 5:
                            //DisplayImage.transform.parent = image_parent_player5.transform;
                            DisplayImage.transform.localPosition = new Vector3(startPosition.x + i5, startPosition.y + j5, 0);
                            DisplayImage.transform.localEulerAngles = new Vector3(0, 0, 0);

                            (i5, j5) = updatePosition(i5, j5);
                            break;
                        default:
                            // DisplayImage.transform.parent = image_parent_player5.transform;
                            DisplayImage.transform.localPosition = new Vector3(startPosition.x + i5, startPosition.y + j5, 0);
                            DisplayImage.transform.localEulerAngles = new Vector3(0, 0, 0);

                            (i5, j5) = updatePosition(i5, j5);
                            break;
                    }

                }
            }

            

        }


        public (float i, float j) updatePosition(float i, float j)
        {

            if (i < 1.20)  //set up new location
            {
                i = i + 0.22f;// 5*0.23 = 1.15
            }
            else
            {
                i = 0;
                j = j - 0.18f;
            }

            return (i, j);
        }

        public void DestroyAllCubes()
        {
            foreach (GameObject cube in previousCubes)
            {
                //PhotonNetwork.Destroy(cube);
            }
            previousCubes.Clear();
        }

        [PunRPC]
        public void GoToSceen(string scene)
        {
            DebugLog.text = "click into GoToSceen()";
            LogSSnumber.LogScreenshot();
            SceneManager.LoadScene(GoToScene2);
        }

  
        [PunRPC]
        public void RPC_showimage(string n)
        {
            DebugLogSS.text = "number:" + n;

            GameObject[] genericCube = GameObject.FindGameObjectsWithTag("GenericCube");

            DebugLog.text = "screenshot_store:" + screenshot_store.Count;

            DebugLogSS.text = "generic Cube:" + genericCube.Length;

            int m = 0;
            if (screenshot_store[m] != null)
            {
                genericCube[m].GetComponentInChildren<RawImage>().texture = screenshot_store[m].image;
            }
            else
            {
                Debug.Log("list is null!!!!");
            }

        }

        

    }
}



