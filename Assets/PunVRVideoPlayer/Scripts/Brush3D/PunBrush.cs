using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class PunBrush : LoggableTool
{
    enum Hand { Right, Left };
    [SerializeField] Hand hand;

    // Used to keep track of the current brush tip position and the actively drawing brush stroke
    [SerializeField] private Transform _tip;
    [SerializeField] private Transform _lasertip;
    [SerializeField] private Transform _line;

    private BrushStroke _activeBrushStroke;

    //used for log
    private int _colorChangeCount = 0, _strokeCount = 0, _vertexCount = 0;
    public int _strokeCount_discussion = 0;
    public int _strokeCount_video = 0;
    
    private BrushStrokeMesh _brushstrokemesh;
    private int index;
    private string name;
    private float tipsize;

    public GameObject brushStrokeObject;
    public SetLayer setLayer;
    public int playerID;
    //public Text debuglog;
    public Camera _camera;
    ////
    private float t = 0;

    private List<GameObject> _previousStrokes;


    MeshRenderer _tipRenderer;

    void Start()
    {
        //debuglog = GameObject.Find("/Canvas/DebugLog").GetComponent<Text>();
        //_camera = GameObject.Find("/OVRNetworkCameraRig").GetComponent<Camera>();

        _previousStrokes = new List<GameObject>();

        index = SceneManager.GetActiveScene().buildIndex;
        name = SceneManager.GetActiveScene().name;
        playerID = PhotonNetwork.LocalPlayer.ActorNumber;

        if (index == 1)
        {//index == 1 under discussion room
            _tip.gameObject.GetComponent<MeshRenderer>().enabled = true;
            _lasertip.gameObject.GetComponent<MeshRenderer>().enabled = false;
            _line.gameObject.GetComponent<LineRenderer>().enabled = false;
            _tipRenderer = _tip.gameObject.GetComponent<MeshRenderer>();
        }
        else
        {
            _tip.gameObject.GetComponent<MeshRenderer>().enabled = false;
            _lasertip.gameObject.GetComponent<MeshRenderer>().enabled = true;
            _line.gameObject.GetComponent<LineRenderer>().enabled = true;
            _tipRenderer = _lasertip.gameObject.GetComponent<MeshRenderer>();
        }



    }

    void Update()
    {

        if (!photonView.IsMine) return;
       
        if (hand == Hand.Left)
        {
            t = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
        }
        else if (hand == Hand.Right)
        {
            t = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        }
        
        if (t > 0.5f && _activeBrushStroke == null)
        {
            Color strokeColor = _tipRenderer.material.color;
            object[] colorData = { strokeColor.r, strokeColor.g, strokeColor.b, strokeColor.a};
            
            if (index == 3)// non-sync mode
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                {
                    brushStrokeObject = PhotonNetwork.Instantiate("BrushStrokePrefab1", Vector3.zero, Quaternion.identity, 0, colorData);
                }
                else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)

                {
                    brushStrokeObject = PhotonNetwork.Instantiate("BrushStrokePrefab2", Vector3.zero, Quaternion.identity, 0, colorData);
                }
                else if (PhotonNetwork.LocalPlayer.ActorNumber == 3)
                {
                    brushStrokeObject = PhotonNetwork.Instantiate("BrushStrokePrefab3", Vector3.zero, Quaternion.identity, 0, colorData);
                }
                else
                {
                    brushStrokeObject = PhotonNetwork.Instantiate("BrushStrokePrefab", Vector3.zero, Quaternion.identity, 0, colorData);
                }
                
            }else
            {
                brushStrokeObject = PhotonNetwork.Instantiate("BrushStrokePrefab", Vector3.zero, Quaternion.identity, 0, colorData);
            }

            _previousStrokes.Add(brushStrokeObject);
            LogAnnotatioin();

            _activeBrushStroke = brushStrokeObject.GetComponent<BrushStroke>();


            if (index == 1){//discussion room
                _activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(_tip.position, _tip.rotation);
                //_strokeCount_discussion ++;
            }
            else{
                _activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(_lasertip.position, _lasertip.rotation);
                _brushstrokemesh = brushStrokeObject.GetComponent<BrushStrokeMesh>();
                //_strokeCount_video ++;
            }
            
        }

        if (t > 0.5f){

            /*if (index == 3)// non-sync mode
            {
                //debuglog.text = "run into index = 3 &&t > 0.5f";
                brushStrokeObject.GetComponent<SetLayer>().SetLayerRPC(playerID);
            }*/

            if (index == 1){
                _activeBrushStroke.MoveBrushTipToPoint(_tip.position, _tip.rotation);

            }
            else{
                _activeBrushStroke.MoveBrushTipToPoint(_lasertip.position, _lasertip.rotation);
                
            }
            //_activeBrushStroke.MoveBrushTipToPoint(_tip.position, _tip.rotation);
        }

        if (t < 0.5f && _activeBrushStroke != null)
        {

            if (index == 1){
                _activeBrushStroke.EndBrushStrokeWithBrushTipPoint(_tip.position, _tip.rotation);
                _vertexCount += _activeBrushStroke.GetVertexCount();
                _activeBrushStroke = null;

            }
            else{
                _activeBrushStroke.EndBrushStrokeWithBrushTipPoint(_lasertip.position, _lasertip.rotation);
                _vertexCount += _activeBrushStroke.GetVertexCount();
                _activeBrushStroke = null;
            }

            


        }

    }

    public void UndoPreviousStroke()
    {
        if (_previousStrokes.Count == 0 || _activeBrushStroke != null) return;

        GameObject previousBrushStroke = _previousStrokes[_previousStrokes.Count - 1];
        PhotonNetwork.Destroy(previousBrushStroke);

        _previousStrokes.RemoveAt(_previousStrokes.Count-1);
    }

    public void ClearAllStrokes()
    {
        foreach (GameObject stroke in _previousStrokes)
        {
            PhotonNetwork.Destroy(stroke);
        }
        _previousStrokes.Clear();
    }

    public void OnColorPress(string htmlValue)
    {
        this._colorChangeCount++;
        this.photonView.RPC("ChangeBrushColor", RpcTarget.All, htmlValue);
;   }

    [PunRPC]
    public void ChangeBrushColor(string htmlValue)
    {
        Color newCol;
        if (ColorUtility.TryParseHtmlString(htmlValue, out newCol))
            _tipRenderer.material.color = newCol;
    }

    public void LogAnnotatioin()
    {
        string path = Application.persistentDataPath + "/RoomLog.log";
        using FileStream stream = new FileStream(path, FileMode.Append);
        using var sr = new StreamWriter(stream);

        sr.WriteLineAsync("A");
    }

}
