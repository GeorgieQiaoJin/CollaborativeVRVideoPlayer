using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class PunMarker : MonoBehaviourPun
{
    [SerializeField]
    private Transform _tip;
    [SerializeField]
    public int _penSize = 5;

    private Renderer renderer;
    public Color[] colors;
    private float tipHeight;

    private RaycastHit touch;
    private Whiteboard whiteboard;
    private Vector2 touchPos;
    public bool touchedLastFrame;

    private Vector2 lastTouchPos;
    private Quaternion lastTouchRot;

    private List<Vector2> strokePoints;

    private void Start()
    {
        renderer = _tip.GetComponent<Renderer>();
        colors = Enumerable.Repeat(renderer.material.color, _penSize * _penSize).ToArray();
        tipHeight = _tip.localScale.y + _tip.localPosition.y;
        strokePoints = new List<Vector2>();
    }

    private void Update()
    {
        Draw();
    }

    private void BrushAreaWithColor(int x, int y, Vector2 startPos)
    {
        // Color in point at tip
        whiteboard.texture.SetPixels(x, y, _penSize, _penSize, colors); //whiteboard is null

        // Interpolate (think about dragging a marker fast across the board)
        for (float f = 0.01f; f < 1f; f += 0.03f)
        {
            var lerpX = (int)Mathf.Lerp(startPos.x, x, f);
            var lerpY = (int)Mathf.Lerp(startPos.y, y, f);

            whiteboard.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, colors);
        }

        whiteboard.texture.Apply();  
    }

    private void SendBrushStroke()
    {
        //Stroke has just ended if touchedLastFrame was true at this point
        if (touchedLastFrame && strokePoints.Count > 0)
        {
            Vector2[] stroke = strokePoints.ToArray();
            whiteboard.photonView.RPC("BrushStroke", RpcTarget.OthersBuffered, photonView.ViewID, (object)stroke);

            strokePoints.Clear();
            Debug.Log("Finished stroke");
        }

    }

    private void Draw()
    {
        if (Physics.Raycast(_tip.position, transform.up, out touch, tipHeight))
        {
            if (!touch.transform.CompareTag("Whiteboard"))
            {
                SendBrushStroke();
                whiteboard = null;
                touchedLastFrame = false;
            }
            else {
                if (whiteboard == null)
                {
                    whiteboard = touch.transform.GetComponent<Whiteboard>();
                    Debug.Log("Hit whiteboard");
                }

                touchPos = touch.textureCoord;

                var x = (int)(touchPos.x * whiteboard.textureSize.x - (_penSize / 2));
                var y = (int)(touchPos.y * whiteboard.textureSize.y - (_penSize / 2));

                if (y < 0 || y > whiteboard.textureSize.y || x < 0 || x > whiteboard.textureSize.x) return;
                
                if (touchedLastFrame){
                    BrushAreaWithColor(x, y, lastTouchPos);
                    // transform.rotation = lastTouchRot; //Prevent clipping issues when hitting wall, needs adjustment
                }
                lastTouchRot = transform.rotation;
                lastTouchPos = new Vector2(x, y);
                touchedLastFrame = true;

                strokePoints.Add(lastTouchPos);
            }
        } 
        else
        {
            SendBrushStroke();
            whiteboard = null;
            touchedLastFrame = false;
        }
    }
}
