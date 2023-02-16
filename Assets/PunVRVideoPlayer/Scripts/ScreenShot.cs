using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScreenShot
{
    public Texture2D image;
    public float progress;
    public float positionX;
    public bool Displayed;
    public int level;
    public Color playerColor;
    public int playerID;
    public string note;

    public ScreenShot(Texture2D Image, int PlayerID, float Progress)
    {
        this.image = Image;
        this.progress = Progress;
        this.positionX = 0.0f;
        this.Displayed = false;
        this.level = 0; // 0 for non-displayed screenshots
        this.playerID = PlayerID;
        this.note = "";
        Color32 red = new Color32(199, 59, 11, 255);
        Color32 blue = new Color32(86, 180, 233, 255);
        Color32 yellow = new Color32(239, 199, 132, 255);
        Color32 green = new Color32(37, 190, 103, 255);
        Color32 orange = new Color32(230, 159, 0, 255);


        switch (PlayerID)
        {
            case 1:
                this.playerColor = orange;// Color.red;
                break;
            case 2:
                this.playerColor = blue;//Color.cyan;
                break;
            case 3:
                this.playerColor = green;// Color.green;
                break;
            case 4:
                this.playerColor = Color.yellow;
                break;
            case 5:
                this.playerColor = Color.magenta;
                break;
            default:
                this.playerColor = Color.blue;
                break;
        }
    }
}
