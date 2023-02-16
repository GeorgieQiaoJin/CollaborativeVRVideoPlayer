using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LoggableTool : MonoBehaviourPun
{
    protected float _timeActive = 0;
    public virtual void Update()
    {
        if (photonView.IsMine)
            _timeActive += Time.deltaTime;
    }
    public virtual string SendLogInfo()
    {
        return string.Format("Time {0} was active: {1}\n", gameObject.name, _timeActive);
    }
}
