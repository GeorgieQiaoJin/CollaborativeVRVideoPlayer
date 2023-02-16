using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Networking.Pun2
{
	public class GoToVideo : MonoBehaviourPunCallbacks
	{
		public GameObject sphere;
		public string GoToScene;

		// Start is called before the first frame update
		void Start()
	    {
// PhotonNetwork.AutomaticallySyncScene = true;
		}

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

	    private void OnTriggerEnter(Collider other)
		{
			Debug.Log("Trigger Enter");

			//Get the Renderer component from the new cube
			var cubeRenderer = sphere.GetComponent<Renderer>();

			//Call SetColor using the shader property name "_Color" and setting the color to red
			cubeRenderer.material.SetColor("_Color", Color.red);

			// SceneManager.LoadScene(2); //go to the room scene
			if (PhotonNetwork.IsMasterClient)
			{
				//SceneManager.LoadScene("1Photon2Room");
				this.photonView.RPC("GoToSceen", RpcTarget.All, GoToScene);
				Debug.Log("run into ShowRoom2");
				//this.photonView.RPC("ShowRoom2", RpcTarget.All);
				

			}
			cubeRenderer.material.SetColor("_Color", Color.green);
		}

		[PunRPC]
		public void GoToSceen(string scene)
        {
			PhotonNetwork.DestroyAll();
			SceneManager.LoadScene(scene);
		}


	}
}
