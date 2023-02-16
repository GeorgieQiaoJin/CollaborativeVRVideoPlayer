using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class RecordStatusIndicator : MonoBehaviourPunCallbacks
{
    public int IconID;
    public Color playerColor;
    public Color touColor;
    public GameObject head;
    public Networking.Pun2.PersonalManager personalManager;
    public int playerid;
    public int muteid;
    public Text _debug;

    // Start is called before the first frame update
    void Start()
    {
        /*
        Color red = new Color(220, 38, 127);
        Color blue = new Color(100, 143, 255);
        Color purple = new Color(120, 94, 240);
        Color orange = new Color(254, 97, 0);
        Color yellow = new Color(255, 176, 0);*/
        /*
        GameObject[] heads = GameObject.FindGameObjectsWithTag("Head");

        foreach (GameObject head in heads)
        {
            if(head.GetComponent<PhotonView>().IsMine){
                Debug.Log("headid:-----"+ head.GetComponent<PhotonView>().ViewID);
            }
          
        }*/

        playerid = PhotonNetwork.LocalPlayer.ActorNumber;

        Color32 red = new Color32(199, 59, 11, 255);
        Color32 blue = new Color32(86, 180, 233, 255);
        Color32 yellow = new Color32(239, 199, 132, 255);
        Color32 green = new Color32(37, 190, 103, 255);
        Color32 orange = new Color32(230, 159, 0, 255);


        switch (IconID)
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
                playerColor = Color.blue;
                break;
        }

        //this.GetComponent<SpriteRenderer>().color = playerColor;
        touColor = new Color32(255, 255, 255, 0);

        this.GetComponent<SpriteRenderer>().color = touColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRecording()
    {
        _debug.text = "after iconREC- StartRecording";
        muteid = personalManager.getheadid();
        _debug.text = "MUTEID-start:" + muteid;

        if (PhotonNetwork.LocalPlayer.ActorNumber == IconID)
        {
            this.photonView.RPC("ChangeStatus", RpcTarget.All, true);
            
        }

        this.photonView.RPC("Mute", RpcTarget.All, muteid, true);



    }

    public void StopRecording()
    {
        _debug.text = "after iconREC- StopRecording";
        muteid = personalManager.getheadid();
        _debug.text = "MUTEID-stop:" + muteid;
        //Debug.Log("MUTEID-stop:" + muteid);

        if (PhotonNetwork.LocalPlayer.ActorNumber == IconID)
        {
            this.photonView.RPC("ChangeStatus", RpcTarget.All, false);
            
        }

        this.photonView.RPC("Mute", RpcTarget.All, muteid, false);

    }


    [PunRPC]
    public void ChangeStatus(bool active)
    {
        if (active)
        {
            this.GetComponent<SpriteRenderer>().color = playerColor;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = touColor;
        }
            
        //this.gameObject.SetActive(active);
    }

    [PunRPC]
    public void Mute(int muteid, bool active)
    {
        
        if (active)
        {
            //Debug.Log("Mute!!");
            PhotonView.Find(muteid).gameObject.GetComponent<AudioSource>().mute = true;
        }
        else
        {
            //Debug.Log("Un-Mute!!");
            PhotonView.Find(muteid).gameObject.GetComponent<AudioSource>().mute = false;
        }
        
        

    }


}
