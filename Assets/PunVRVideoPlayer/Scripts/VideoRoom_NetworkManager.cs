using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Networking.Pun2
{
	public class VideoRoom_NetworkManager : MonoBehaviourPunCallbacks
	{
		//////////////////////////////////////////////////////////////////////////////////////////

        #region Public Methods

        [Tooltip("The prefab for control video playback")]
		public GameObject videocontrol_prefab;

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

		//////////////////////////////////////////////////////////////////////////////////////////

        #region MonoBehaviour CallBacks

	    // Start is called before the first frame update
	    void Start()
	    {
	    }

	    // Update is called once per frame
	    void Update()
	    {
	    	
	    }

	    #endregion

		//////////////////////////////////////////////////////////////////////////////////////////

	    #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

	    #endregion
	}
}
