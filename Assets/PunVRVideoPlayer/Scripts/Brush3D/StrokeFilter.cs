using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class StrokeFilter : MonoBehaviour
{
    [SerializeField] Camera _camera;
    int playerId;
    int deleteLayer;

    // Start is called before the first frame update
    void Start()
    {
        playerId = PhotonNetwork.LocalPlayer.ActorNumber;
        LayerFilter(playerId);
     }

    public void LayerFilter(int playerId)
    {
     
        switch (playerId)
        {
            case 1://keep 8
                _camera.cullingMask &= ~(1 << 9);//delete layer based on playerID;
                _camera.cullingMask &= ~(1 << 10);
                break;
            case 2://keep 9
                _camera.cullingMask &= ~(1 << 8);
                _camera.cullingMask &= ~(1 << 10);
                break;
            case 3://keep 10
                _camera.cullingMask &= ~(1 << 8);
                _camera.cullingMask &= ~(1 << 9);
                break;
            default:
                _camera.cullingMask &= ~(1 << 8);
                _camera.cullingMask &= ~(1 << 9);
                break;

        }
    }

    public void DeleteFilter(object[] layers)
    {
        int whoisfollowing_layer = (int)layers[0];
        int whoisfollowed_layer = (int)layers[1];


        _camera.cullingMask |= (1 << whoisfollowing_layer);
        _camera.cullingMask |= (1 << whoisfollowed_layer); //based on the prior layers, add x
        //_camera.cullingMask |= (1 << whoisfollowed_layer); //based on the prior layers, add x
    }

    public void ShowAllLayers()
    {
        _camera.cullingMask |= (1 << 8);
        _camera.cullingMask |= (1 << 9);
        _camera.cullingMask |= (1 << 10);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
