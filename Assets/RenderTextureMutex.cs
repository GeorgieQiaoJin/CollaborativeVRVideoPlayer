using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureMutex : MonoBehaviour
{
    public bool RenderTextureInUse;
    // Start is called before the first frame update
    void Start()
    {
        RenderTextureInUse = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
