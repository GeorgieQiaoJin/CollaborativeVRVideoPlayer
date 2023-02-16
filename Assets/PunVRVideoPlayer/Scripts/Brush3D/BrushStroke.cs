using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEditor;

public class BrushStroke : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback
{
    [SerializeField]
    private BrushStrokeMesh _mesh;

    // Ribbon State
    struct RibbonPoint
    { // Make sure these are synchronized
        public Vector3 position;
        public Quaternion rotation;
    }

    /////
    private int index;
    private string name;
    private float strokesize = 0.02f;
    ///

    private List<RibbonPoint> _ribbonPoints = new List<RibbonPoint>(); 
    private Vector3 _brushTipPosition;
    private Quaternion _brushTipRotation;
    private bool _brushStrokeFinalized;

    // Smoothing
    private Vector3 _ribbonEndPosition;
    private Quaternion _ribbonEndRotation = Quaternion.identity;

    // Mesh
    private Vector3 _previousRibbonPointPosition;
    private Quaternion _previousRibbonPointRotation = Quaternion.identity;

    void Start()
    {
        index = SceneManager.GetActiveScene().buildIndex;
        //name = SceneManager.GetActiveScene().name;

    }

    // Unity Events
    private void Update()
    {
        // Animate the end of the ribbon towards the brush tip
        AnimateLastRibbonPointTowardsBrushTipPosition();

        // Add a ribbon segment if the end of the ribbon has moved far enough
        AddRibbonPointIfNeeded();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.IsWriting)
        {
            // Synchronize ribbon points (array of positions and rotations)
            stream.SendNext(_brushTipPosition);
            stream.SendNext(_brushTipRotation);
            stream.SendNext(_brushStrokeFinalized);
        }
        else
        {
            _brushTipPosition = (Vector3)stream.ReceiveNext();
            _brushTipRotation = (Quaternion)stream.ReceiveNext();
            _brushStrokeFinalized = (bool)stream.ReceiveNext();
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        Color sentColor = new Color((float)data[0], (float)data[1], (float)data[2], (float)data[3]);
        _mesh.GetComponent<MeshRenderer>().material.color = sentColor;
    }

    // Interface
    public void BeginBrushStrokeWithBrushTipPoint(Vector3 position, Quaternion rotation)
    {
        // Update the model
        _brushTipPosition = position;
        _brushTipRotation = rotation;
        // Update last ribbon point to match brush tip position & rotation
        _ribbonEndPosition = position;
        _ribbonEndRotation = rotation;

        switch (index)
        {
            case 1:
                _mesh.ChangeStrokeSize(0.02f);
                break;
            case 2:
                _mesh.ChangeStrokeSize(0.5f);//0.7
                break;
            case 3:
                _mesh.ChangeStrokeSize(0.5f);//0.7
                break;
            default:
                _mesh.ChangeStrokeSize(0.02f);
                break;
        }
        /*
        if (index != 1){
                _mesh.ChangeStrokeSize (0.7f);
        } else if (index == 1)
        {
            _mesh.ChangeStrokeSize(0.02f);
        }
        */

        _mesh.UpdateLastRibbonPoint(_ribbonEndPosition, _ribbonEndRotation);
    }

    public void MoveBrushTipToPoint(Vector3 position, Quaternion rotation)
    {
        _brushTipPosition = position;
        _brushTipRotation = rotation;
    }

    public void EndBrushStrokeWithBrushTipPoint(Vector3 position, Quaternion rotation)
    {
        // Add a final ribbon point and mark the stroke as finalized
        this.photonView.RPC("RpcAddRibbonPoint", RpcTarget.All, position, rotation);
        _brushStrokeFinalized = true;
    }


    // Ribbon drawing
    private void AddRibbonPointIfNeeded()
    {
        // If the brush stroke is finalized, stop trying to add points to it.
        if (!photonView.IsMine || _brushStrokeFinalized)
            return;

        if (Vector3.Distance(_ribbonEndPosition, _previousRibbonPointPosition) >= 0.01f ||
            Quaternion.Angle(_ribbonEndRotation, _previousRibbonPointRotation) >= 10.0f)
        {

            //// Add ribbon point model to ribbon points array. This will fire the RibbonPointAdded event to update the mesh.
            //AddRibbonPoint(_ribbonEndPosition, _ribbonEndRotation);

            this.photonView.RPC("RpcAddRibbonPoint", RpcTarget.AllBuffered, _ribbonEndPosition, _ribbonEndRotation);//, PhotonNetwork.LocalPlayer.ActorNumber

            // Store the ribbon point position & rotation for the next time we do this calculation
            _previousRibbonPointPosition = _ribbonEndPosition;
            _previousRibbonPointRotation = _ribbonEndRotation;
        }
    }

    [PunRPC]
    public void RpcAddRibbonPoint(Vector3 position, Quaternion rotation)//, int localActorNumber
    {
        
        
        // Create the ribbon point
        RibbonPoint ribbonPoint = new RibbonPoint();
        ribbonPoint.position = position;
        ribbonPoint.rotation = rotation;
        _ribbonPoints.Add(ribbonPoint);
        //_mesh.ChangeStrokeSize(0.02f);//0.7
        
        
        switch (index)
        {
            case 1:
                _mesh.ChangeStrokeSize(0.02f);
                break;
            case 2:
                _mesh.ChangeStrokeSize(0.5f);//0.7
                break;
            case 3:
                _mesh.ChangeStrokeSize(0.5f);//0.7
                break;
            default:
                _mesh.ChangeStrokeSize(0.02f);
                break;
        }
          /*
        if (index == 2 || index == 2)
        {
            // Create the ribbon point
            RibbonPoint ribbonPoint = new RibbonPoint();
            ribbonPoint.position = position;
            ribbonPoint.rotation = rotation;
            _ribbonPoints.Add(ribbonPoint);
            _mesh.ChangeStrokeSize (0.7f);//0.7
            // Update the mesh
            _mesh.InsertRibbonPoint(position, rotation);

        }
        else if (index == 1)//discussion room index = 1
        {// Create the ribbon point
            RibbonPoint ribbonPoint = new RibbonPoint();
            ribbonPoint.position = position;
            ribbonPoint.rotation = rotation;
            _ribbonPoints.Add(ribbonPoint);
            _mesh.ChangeStrokeSize(0.02f);
            // Update the mesh
            _mesh.InsertRibbonPoint(position, rotation);
        }
        */

        // Update the mesh
        _mesh.InsertRibbonPoint(position, rotation);

    }

    // Brush tip + smoothing
    private void AnimateLastRibbonPointTowardsBrushTipPosition()
        {
            // If the brush stroke is finalized, skip the brush tip mesh, and stop animating the brush tip.
            if (_brushStrokeFinalized)
            {
                _mesh.skipLastRibbonPoint = true;
                return;
            }

            Vector3 brushTipPosition = _brushTipPosition;
            Quaternion brushTipRotation = _brushTipRotation;

            // If the end of the ribbon has reached the brush tip position, we can bail early.
            if (Vector3.Distance(_ribbonEndPosition, brushTipPosition) <= 0.0001f &&
                Quaternion.Angle(_ribbonEndRotation, brushTipRotation) <= 0.01f)
            {
                return;
            }

            // Move the end of the ribbon towards the brush tip position
            _ribbonEndPosition = Vector3.Lerp(_ribbonEndPosition, brushTipPosition, 25.0f * Time.deltaTime);
            _ribbonEndRotation = Quaternion.Slerp(_ribbonEndRotation, brushTipRotation, 25.0f * Time.deltaTime);

            // Update the end of the ribbon mesh
            _mesh.UpdateLastRibbonPoint(_ribbonEndPosition, _ribbonEndRotation);
        }


    [PunRPC]
    public void UpdateMeshColor(Color newColor)
    {
        _mesh.GetComponent<MeshRenderer>().material.color = newColor;
    }

    public int GetVertexCount() { return _mesh.GetVertexCount(); }
}