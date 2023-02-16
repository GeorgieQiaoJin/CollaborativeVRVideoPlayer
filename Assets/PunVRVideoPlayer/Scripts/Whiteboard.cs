using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Whiteboard : MonoBehaviourPunCallbacks, IPunObservable
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 2048);

    private void Start()
    {
        texture = new Texture2D((int)textureSize.x, (int) textureSize.y);
        GetComponent<Renderer>().material.mainTexture = texture;
    }

    public byte[] GetAllTextureData()
    {
        return texture.GetRawTextureData();
    }
    public void SetAllTextureData(byte[] textureData)
    {
        texture.LoadRawTextureData(textureData);
        texture.Apply();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(texture);
        }
        else
        {
            texture = (Texture2D)stream.ReceiveNext();
            texture.Apply();
        }
    }


    [PunRPC]
    public void BrushAreaWithColor(int x, int y, Vector2 startPos, int brushId)
    {
        PunMarker pb = PhotonView.Find(brushId).GetComponent<PunMarker>();
        int penSize = pb._penSize;
        Color[] colors = pb.colors;

        // Color in point at tip
        texture.SetPixels(x, y, penSize, penSize, colors);

        // Interpolate (think about dragging a marker fast across the board)
        for (float f = 0.01f; f < 1f; f += 0.03f)
        {
            var lerpX = (int)Mathf.Lerp(startPos.x, x, f);
            var lerpY = (int)Mathf.Lerp(startPos.y, y, f);

            texture.SetPixels(lerpX, lerpY, penSize, penSize, colors);
        }

        texture.Apply();

    }


    [PunRPC]
    public void BrushStroke(int brushId, Vector2[] points)
    {
        PunMarker pb = PhotonView.Find(brushId).GetComponent<PunMarker>();
        int penSize = pb._penSize;
        Color[] colors = pb.colors;

        for (int i=1; i< points.Length; i++)
        {
            Vector2 startPos = points[i - 1];
            int x = (int) points[i].x;
            int y = (int) points[i].y;

            // Color in point at tip
            texture.SetPixels(x, y, penSize, penSize, colors);

            // Interpolate (think about dragging a marker fast across the board)
            for (float f = 0.01f; f < 1f; f += 0.03f)
            {
                var lerpX = (int)Mathf.Lerp(startPos.x, x, f);
                var lerpY = (int)Mathf.Lerp(startPos.y, y, f);

                texture.SetPixels(lerpX, lerpY, penSize, penSize, colors);
            }
        }
        

        texture.Apply();

    }
}
