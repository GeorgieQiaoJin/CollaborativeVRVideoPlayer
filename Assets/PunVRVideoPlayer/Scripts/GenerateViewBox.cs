using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GenerateViewBox : MonoBehaviourPunCallbacks
{
    public GameObject pb_ViewBox;
    public GameObject pb_ViewArrow;
    public GameObject ViewBox;
    public GameObject ViewArrowCanvas;
    public GameObject UICam;
    public float arrowcanvas_width;
    public float arrowcanvas_height;
    public RawImage ViewArrow;
    public GameObject REC_Icon;
    public float distanceFromCamera;
    public Color playerColor;
    public int PlayerID;
    public GameObject MainCam;
    public Text DebugLog;
    public bool Mod_NSync;

    // Start is called before the first frame update
    void Start()
    {
        MainCam = GameObject.Find("/OVRNetworkCameraRig/TrackingSpace/CenterEyeAnchor");
        DebugLog = GameObject.Find("/Canvas/DebugLog").GetComponent<Text>();
        UICam = GameObject.Find("/OVRNetworkCameraRig/TrackingSpace/CenterEyeAnchor/UICamera");

        Vector3 forward_vector = transform.forward;
        Vector3 origin_position = transform.position;
        Vector3 resultingPosition = origin_position + forward_vector * distanceFromCamera;

        ViewBox = Instantiate(pb_ViewBox);
        ViewBox.transform.position = resultingPosition;

        ViewArrowCanvas = Instantiate(pb_ViewArrow);
        ViewArrowCanvas.GetComponent<Canvas>().worldCamera = UICam.GetComponent<Camera>();
        ViewArrow = ViewArrowCanvas.transform.GetChild(0).GetComponent<RawImage>();
        //ViewArrow.transform.position = resultingPosition;
        arrowcanvas_width = ViewArrowCanvas.transform.GetComponent<RectTransform>().rect.width;
        arrowcanvas_height = ViewArrowCanvas.transform.GetComponent<RectTransform>().rect.height;
        arrowcanvas_width *= 0.5f;
        arrowcanvas_height *= 0.5f;
        arrowcanvas_width -= 400f;
        arrowcanvas_height -= 400f;

        Quaternion rotation = transform.rotation;
        ViewBox.transform.rotation = rotation;
        //ViewArrow.transform.rotation = rotation;

        if (photonView.IsMine)
        {
            PlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
            this.photonView.RPC("SetColor", RpcTarget.All, PlayerID);
        }
        ViewBox.transform.GetChild(0).GetComponent<RawImage>().color = playerColor;
        ViewArrow.color = playerColor;
        REC_Icon = ViewBox.transform.Find("ViewBox/REC").gameObject;
        REC_Icon.GetComponent<RawImage>().color = playerColor;
        REC_Icon.SetActive(false);

        if (photonView.IsMine || Mod_NSync)
        {
            ViewBox.SetActive(false);
            ViewArrowCanvas.SetActive(false);
        }
    }

    private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        Vector2 diference = vec2 - vec1;
        float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, diference) * sign;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mod_NSync) return;

        // viewbox section
        Vector3 forward_vector = transform.forward;
        Vector3 origin_position = transform.position;
        Vector3 resultingPosition = origin_position + forward_vector * distanceFromCamera;

        ViewBox.transform.position = resultingPosition;

        Quaternion rotation = transform.rotation;
        ViewBox.transform.rotation = rotation;

        // viewarrow section
        // first decide the angle between main user and display user and find that if the viewarrow should be shown
        Vector3 main_forward = MainCam.transform.forward;
        float angle = Vector3.Angle(main_forward, forward_vector);

        if (angle > 60.0f)
        {
            ViewArrowCanvas.SetActive(true);
        }
        else
        {
            ViewArrowCanvas.SetActive(false);
        }

        // next find the yaw and pitch of main cam and display user:
        Quaternion q = this.transform.rotation;
        float Pitch_view = Mathf.Rad2Deg * Mathf.Atan2(2 * q.x * q.w - 2 * q.y * q.z, 1 - 2 * q.x * q.x - 2 * q.z * q.z) + 180.0f;
        float Yaw_view = Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z) + 180.0f;

        q = MainCam.transform.rotation;
        float Pitch_user = Mathf.Rad2Deg * Mathf.Atan2(2 * q.x * q.w - 2 * q.y * q.z, 1 - 2 * q.x * q.x - 2 * q.z * q.z) + 180.0f;
        float Yaw_user = Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z) + 180.0f;

        if (Mathf.Abs(Yaw_user - Yaw_view) > 180)
        {
            if (Yaw_user > Yaw_view)
            {
                Yaw_user -= 360.0f;
            }
            else
            {
                Yaw_view -= 360.0f;
            }
        }

        float arrowangle = AngleBetweenVector2(new Vector2(Yaw_view, Pitch_view), new Vector2(Yaw_user, Pitch_user));

        //DebugLog.text = "view yaw " + Yaw_view + ", pitch " + Pitch_view + "\n"
        //    + "user yaw " + Yaw_user + ", pitch " + Pitch_user + "\n"
        //    + "angle " + arrowangle;

        RectTransform rectTransform = ViewArrow.GetComponent<RectTransform>();
        Vector3 arrowRotation = rectTransform.eulerAngles;
        arrowangle = -(arrowangle + 90.0f);
        arrowRotation.z = arrowangle;
        rectTransform.eulerAngles = arrowRotation;

        Vector3 arrowPosition = ViewArrow.transform.localPosition;
        // calcuate the view arrow's position:
        if (Yaw_user > Yaw_view)
        {
            // user is on the right?
            arrowPosition.x = -arrowcanvas_width;
        }
        else
        {
            arrowPosition.x = arrowcanvas_width;
        }
        ViewArrow.transform.localPosition = arrowPosition;

        if (photonView.IsMine)
        {
            ViewArrowCanvas.SetActive(false);
        }

    }

    public void SetFollowMode(bool follow)
    {
        if (photonView.IsMine) return;

        Mod_NSync = !follow;

        if (Mod_NSync)
        {
            ViewBox.SetActive(false);
            ViewArrowCanvas.SetActive(false);
        }
        else
        {
            ViewBox.SetActive(true);
            ViewArrowCanvas.SetActive(true);
        }

    }

    public void ShowREC()
    {
        if (photonView.IsMine)
        {
            this.photonView.RPC("RPC_SetREC", RpcTarget.Others, true);
            //REC_Icon.SetActive(true);
        }
    }

    public void HideREC()
    {
        if (photonView.IsMine)
        {
            this.photonView.RPC("RPC_SetREC", RpcTarget.Others, false);
            //REC_Icon.SetActive(false);
        }
    }

    [PunRPC]
    public void SetColor(int playerID)
    {
        Color32 red = new Color32(199, 59, 11, 255);
        Color32 blue = new Color32(86, 180, 233, 255);
        Color32 yellow = new Color32(239, 199, 132, 255);
        Color32 green = new Color32(37, 190, 103, 255);
        Color32 orange = new Color32(230, 159, 0, 255);
        //add to researcher
        Color32 transparent = new Color32(230, 159, 0, 15);

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
                playerColor = transparent;
                break;
            case 5:
                playerColor = transparent;
                break;
            default:
                playerColor = Color.black;
                break;
        }
        ViewBox.transform.GetChild(0).GetComponent<RawImage>().color = playerColor;
        ViewArrow.color = playerColor;
        REC_Icon = ViewBox.transform.Find("ViewBox/REC").gameObject;
        REC_Icon.GetComponent<RawImage>().color = playerColor;
    }

    [PunRPC]
    public void RPC_SetREC(bool active)
    {
        REC_Icon.SetActive(active);
    }

}