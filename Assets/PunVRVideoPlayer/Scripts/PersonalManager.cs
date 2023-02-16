using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//
//For handling local objects and sending data over the network
//
namespace Networking.Pun2
{
    public class PersonalManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] GameObject headPrefab;
        [SerializeField] GameObject handRPrefab;
        [SerializeField] GameObject handLPrefab;
        [SerializeField] GameObject ovrCameraRig;
        [SerializeField] GameObject saveCamPrefab;
        [SerializeField] bool isRightHanded;
        [SerializeField] GameObject[] saveCamPrefabNsync;
        [SerializeField] Transform[] spawnPoints;

        //Tools
        List<GameObject> toolsR;
        List<GameObject> toolsL;
        int currentToolR;
        int currentToolL;
        float swapTimer = 0;

        GameObject head;
        int currentColor;
        System.DateTime startTime;

        public bool NsyncMode;
        public string name;
        

        private void Awake()
        {
            /// If the game starts in Room scene, and is not connected, sends the player back to Lobby scene to connect first.
            if (!PhotonNetwork.NetworkingClient.IsConnected)
            {
                SceneManager.LoadScene("Photon2Lobby");
                return;
            }

           // PhotonNetwork.RejoinRoom("room1");
            Debug.Log("current room name-room1: " + PhotonNetwork.CurrentRoom.Name);
            /////////////////////////////////

            toolsR = new List<GameObject>();
            toolsL = new List<GameObject>();

            if(PhotonNetwork.LocalPlayer.ActorNumber <= spawnPoints.Length)
            {
                ovrCameraRig.transform.position = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position;
                ovrCameraRig.transform.rotation = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.rotation;
            }
        }

        private void Start()
        {
            string name = SceneManager.GetActiveScene().name;

            //Instantiate Head
            GameObject obj = (PhotonNetwork.Instantiate(headPrefab.name, OculusPlayer.instance.head.transform.position, OculusPlayer.instance.head.transform.rotation, 0));
            obj.GetComponent<SetColor>().SetColorRPC(PhotonNetwork.LocalPlayer.ActorNumber, NsyncMode);
            currentColor = PhotonNetwork.LocalPlayer.ActorNumber;
            head = obj;

            //Instantiate saveCam
            // syn mode
            if (saveCamPrefab != null)
            {
                GameObject Instantiate = PhotonNetwork.Instantiate(saveCamPrefab.name, OculusPlayer.instance.head.transform.position, OculusPlayer.instance.head.transform.rotation, 0);
            }

            // non-sync mode:
            if (saveCamPrefabNsync.Length > 0)
            {
                GameObject Instantiate = PhotonNetwork.Instantiate(saveCamPrefabNsync[PhotonNetwork.LocalPlayer.ActorNumber - 1].name, OculusPlayer.instance.head.transform.position, OculusPlayer.instance.head.transform.rotation, 0);
            }

            //Instantiate right hand
            if (isRightHanded)
            {
                obj = (PhotonNetwork.Instantiate(handRPrefab.name, OculusPlayer.instance.rightHand.transform.position, OculusPlayer.instance.rightHand.transform.rotation, 0));
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    toolsR.Add(obj.transform.GetChild(i).gameObject);
                    obj.transform.GetComponentInChildren<SetColor>().SetColorRPC(PhotonNetwork.LocalPlayer.ActorNumber, NsyncMode);
                    if (i > 0)
                        toolsR[i].transform.parent.GetComponent<PhotonView>().RPC("DisableTool", RpcTarget.AllBuffered, i, false);
                }

                // Instantiate left hand with no tools
                obj = (PhotonNetwork.Instantiate("PunHandL", OculusPlayer.instance.leftHand.transform.position, OculusPlayer.instance.leftHand.transform.rotation, 0));
                obj.GetComponent<SetColor>().SetColorRPC(PhotonNetwork.LocalPlayer.ActorNumber, NsyncMode);
                toolsL.Add(obj);
            }
            else
            {
                //Instantiate left hand
                obj = (PhotonNetwork.Instantiate(handLPrefab.name, OculusPlayer.instance.leftHand.transform.position, OculusPlayer.instance.leftHand.transform.rotation, 0));
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    toolsL.Add(obj.transform.GetChild(i).gameObject);
                    obj.transform.GetComponentInChildren<SetColor>().SetColorRPC(PhotonNetwork.LocalPlayer.ActorNumber, NsyncMode);
                    if (i > 0)
                        toolsL[i].transform.parent.GetComponent<PhotonView>().RPC("DisableTool", RpcTarget.AllBuffered, i, false);
                }

                // Instantiate right hand with no tools
                obj = (PhotonNetwork.Instantiate("PunHandR", OculusPlayer.instance.rightHand.transform.position, OculusPlayer.instance.rightHand.transform.rotation, 0));
                obj.GetComponent<SetColor>().SetColorRPC(PhotonNetwork.LocalPlayer.ActorNumber, NsyncMode);
                toolsR.Add(obj);

            }


            LogInteraction(true);

        }

        //Detects input from Thumbstick to switch "hand tools"
        private void Update()
        {
            if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick) && !isRightHanded)
                SwitchToolL();

            if (OVRInput.GetUp(OVRInput.Button.SecondaryThumbstick) && isRightHanded)
                SwitchToolR();

            if (!isRightHanded && Mathf.Abs(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x) > 0.6)
            {
                swapTimer += Time.deltaTime;
                if (swapTimer > 0.2f)
                {
                    swapTimer = 0;
                    SwitchToolL(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < 0);
                }
            }
            else if (isRightHanded && Mathf.Abs(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x) > 0.6)
            {
                swapTimer += Time.deltaTime;
                if (swapTimer > 0.2f)
                {
                    swapTimer = 0;
                    SwitchToolR(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < 0);
                }
            }

        }

        //disables current tool and enables next tool
        void SwitchToolR(bool goToPrev = false)
        {
            name = SceneManager.GetActiveScene().name;

            if (name == "Photon2Room")// discussion room
            {
                toolsR[currentToolR].transform.parent.GetComponent<PhotonView>().RPC("DisableTool", RpcTarget.AllBuffered, currentToolR, false);
                //currentToolR++;
                currentToolR += goToPrev ? toolsR.Count - 1 : 1;
                currentToolR %= toolsR.Count;

                toolsR[currentToolR].transform.parent.GetComponent<PhotonView>().RPC("EnableTool", RpcTarget.AllBuffered, currentToolR, NsyncMode);
            }
            else if (name == "1Photon2RoomBasic")// basic room
            {
                //do nothing
            }else
            {
                toolsR[currentToolR].transform.parent.GetComponent<PhotonView>().RPC("DisableTool", RpcTarget.AllBuffered, currentToolR, false);

                currentToolR += goToPrev ? toolsR.Count - 1 : 1;
                currentToolR %= toolsR.Count;

                //tool 2 is teleport
                if (currentToolR == 2)
                {
                    currentToolR += goToPrev ? toolsR.Count - 1 : 1;
                    currentToolR %= toolsR.Count;
                }
                    

                toolsR[currentToolR].transform.parent.GetComponent<PhotonView>().RPC("EnableTool", RpcTarget.AllBuffered, currentToolR, NsyncMode);

            }

        }

        void SwitchToolL(bool goToPrev = false)
        {
            string name = SceneManager.GetActiveScene().name;

            if (name == "Photon2Room")// discussion room
            {
                toolsL[currentToolL].transform.parent.GetComponent<PhotonView>().RPC("DisableTool", RpcTarget.AllBuffered, currentToolL, false);
                //currentToolR++;
                currentToolL += goToPrev ? toolsL.Count - 1 : 1;
                currentToolL %= toolsL.Count;

                toolsL[currentToolL].transform.parent.GetComponent<PhotonView>().RPC("EnableTool", RpcTarget.AllBuffered, currentToolL, NsyncMode);
            }
            else if (name == "1Photon2RoomBasic")// basic room
            {
                //do nothing
            }
            else
            {
                toolsL[currentToolL].transform.parent.GetComponent<PhotonView>().RPC("DisableTool", RpcTarget.AllBuffered, currentToolL, false);

                currentToolL += goToPrev ? toolsL.Count - 1 : 1;
                currentToolL %= toolsL.Count;

                //tool 2 is teleport
                if (currentToolL == 2)
                {
                    currentToolL += goToPrev ? toolsL.Count - 1 : 1;
                    currentToolL %= toolsL.Count;
                }


                toolsL[currentToolL].transform.parent.GetComponent<PhotonView>().RPC("EnableTool", RpcTarget.AllBuffered, currentToolL, NsyncMode);

            }
        }

        public void SwitchHead(bool isPrev)
        {
            if (isPrev)
                currentColor--;
            else
                currentColor++;

            if (currentColor == 0)
                currentColor = 6;
            if (currentColor == 7)
                currentColor = 1;
            head.GetComponent<SetColor>().SetColorRPC(currentColor, NsyncMode);

            foreach (SetColor sc in toolsL[currentToolL].transform.parent.GetComponentsInChildren<SetColor>(true))
            {
                sc.SetColorRPC(currentColor, NsyncMode);
            }
            foreach (SetColor sc in toolsR[currentToolR].transform.parent.GetComponentsInChildren<SetColor>(true))
            {
                sc.SetColorRPC(currentColor, NsyncMode);
            }
        }

        
        public int getheadid()
        {
            Debug.Log("getheadid----1");
            
            Debug.Log("getheadid"+ head.GetComponent<PhotonView>().ViewID);
            return head.GetComponent<PhotonView>().ViewID;
        }

        public void LogInteraction(bool start, bool hardQuit = false)
        {
            string path = Application.persistentDataPath + "/RoomLog.log";
            using FileStream stream = new FileStream(path, FileMode.Append);
            using var sr = new StreamWriter(stream);

            System.DateTime currentDT = System.DateTime.Now;
            if (start)
            {
                sr.WriteLineAsync("----------------");
                startTime = currentDT;
            }

            sr.WriteAsync(currentDT.ToString("F"));
            if (start && PhotonNetwork.IsMasterClient)
            {
                sr.WriteLineAsync("  MasterClient Created room at scene " + name);
            }
            else if (start)
            {
                sr.WriteLineAsync("  Joined room" + name);
            }
            else
            {
                if (hardQuit)
                {
                    sr.WriteLineAsync("  Left Room");
                    //sr.WriteLineAsync("Number of Play/Pause Button presses: " + videoPlayer.timesPressed.ToString());
                    /*
                    Transform toolHand = (isRightHanded) ? toolsR[currentToolR].transform.parent : toolsL[currentToolL].transform.parent;
                    
                    foreach (var tool in toolHand.GetComponentsInChildren<LoggableTool>(true))
                    {
                        sr.WriteAsync(tool.SendLogInfo());
                    }*/

                }
                else
                {
                    sr.WriteLineAsync("  Paused/Left Room");
                }
                System.TimeSpan diff = currentDT - startTime;
                sr.WriteLineAsync("Elapsed Time since joining:  " + diff.ToString());
            }
        }

        //If disconnected from server, returns to Lobby to reconnect
        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            SceneManager.LoadScene(0);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) LogInteraction(false);
        }

        //So we stop loading scenes if we quit app
        private void OnApplicationQuit()
        {
            StopAllCoroutines();
        }

        public void DisconnectFromLobby()
        {
            LogInteraction(false, true);
            
            PhotonNetwork.Disconnect();
            Application.Quit();
        }
    }
}
