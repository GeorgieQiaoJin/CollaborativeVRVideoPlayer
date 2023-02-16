using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSpawnPoints : MonoBehaviour
{
    public GameObject SpawnPoint;

    // Start is called before the first frame update
    void Start()
    { 
        this.transform.position = new Vector3(SpawnPoint.transform.position.x - 0.6f, SpawnPoint.transform.position.y + 0.6f, SpawnPoint.transform.position.z);
        this.transform.eulerAngles = new Vector3(SpawnPoint.transform.eulerAngles.x, SpawnPoint.transform.eulerAngles.y
                                                , SpawnPoint.transform.eulerAngles.z);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
