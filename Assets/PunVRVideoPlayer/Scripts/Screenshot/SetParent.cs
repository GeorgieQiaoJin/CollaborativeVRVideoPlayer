using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SetParent : MonoBehaviourPunCallbacks
{
    public GameObject MainCanvas;
    public Color playerColor;
    public int PlayerID;
    // Start is called before the first frame update
    void Start()
    {
        MainCanvas = GameObject.Find("Canvas");
        this.transform.SetParent(MainCanvas.transform);
        this.transform.rotation = Quaternion.identity;
        this.transform.localScale = new Vector3(10, 10, 10);

        if (photonView.IsMine)
        {
            PlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
            this.photonView.RPC("SetColor", RpcTarget.All, PlayerID);
        }
        GetComponent<Renderer>().material.color = playerColor; //C sharp
    }

    // Update is called once per frame
    void Update()
    {

    }

    [PunRPC]
    public void SetColor(int playerID)
    {
        Color32 red = new Color32(199, 59, 11, 255);
        Color32 blue = new Color32(86, 180, 233, 255);
        Color32 yellow = new Color32(239, 199, 132, 255);
        Color32 green = new Color32(37, 190, 103, 255);
        Color32 orange = new Color32(230, 159, 0, 255);

        switch (playerID)
        {
            case 1:
                playerColor = orange;
                break;
            case 2:
                playerColor = blue;
                break;
            case 3:
                playerColor = green;
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
        GetComponent<Renderer>().material.color = playerColor; //C sharp
    }
}
