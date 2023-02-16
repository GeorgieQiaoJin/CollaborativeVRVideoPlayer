using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.Video;

namespace Networking.Pun2
{
	public class VideoControl_Sync : MonoBehaviourPun
	{
		//////////////////////////////////////////////////////////////////////////////////////////

        #region Private Fields

        [Tooltip("Video Control Elements")]
        [SerializeField]
        public VideoPlayer vPlayer;
        public bool isPlaying;

        [SerializeField] GameObject handRPrefab;
        [SerializeField] GameObject handLPrefab;

        public void SwitchPlayPause(){
        	isPlaying = !isPlaying;
        }

        #endregion

		//////////////////////////////////////////////////////////////////////////////////////////

        #region MonoBehaviour CallBacks

	    // Start is called before the first frame update
	    void Start()
	    {
	        vPlayer.playOnAwake = false;

            GameObject obj = Instantiate(handRPrefab, OculusPlayer.instance.rightHand.transform.position, OculusPlayer.instance.rightHand.transform.rotation);
            // for (int i = 0; i < obj.transform.childCount; i++)
            // {
            //     toolsR.Add(obj.transform.GetChild(i).gameObject);
            //     obj.transform.GetComponentInChildren<SetColor>().SetColorRPC(PhotonNetwork.LocalPlayer.ActorNumber);
            //     if(i > 0)
            //         toolsR[i].transform.parent.GetComponent<PhotonView>().RPC("DisableTool", RpcTarget.AllBuffered, 1);
            // }
            obj = Instantiate(handLPrefab, OculusPlayer.instance.leftHand.transform.position, OculusPlayer.instance.leftHand.transform.rotation);
            // for (int i = 0; i < obj.transform.childCount; i++)
            // {
            //     toolsL.Add(obj.transform.GetChild(i).gameObject);
            //     obj.transform.GetComponentInChildren<SetColor>().SetColorRPC(PhotonNetwork.LocalPlayer.ActorNumber);
            //     if (i > 0)
            //         toolsL[i].transform.parent.GetComponent<PhotonView>().RPC("DisableTool", RpcTarget.AllBuffered, 1);
            // }
	        // set other video player settings, such as which video, etc, etc
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        if (isPlaying && !vPlayer.isPlaying){
	        	vPlayer.Play();
	        }

	        if (vPlayer.isPlaying && !isPlaying){
	        	vPlayer.Pause();
	        }
	    }

	    #endregion

		//////////////////////////////////////////////////////////////////////////////////////////

		#region IPunObservable implementation

	    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	    {
	    	if (stream.IsWriting)
			{
			    // We own this player: send the others our data
			    stream.SendNext(isPlaying);
			}
			else
			{
			    // Network player, receive data
			    this.isPlaying = (bool)stream.ReceiveNext();
			}
	    }

	    #endregion
	}

}
