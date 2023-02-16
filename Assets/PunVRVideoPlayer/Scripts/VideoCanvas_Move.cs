using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCanvas_Move : MonoBehaviour
{
    public GameObject OculusCamera;
    public float distanceFromCamera;
    public float follow_speed;
    public float canvas_height;
    // Start is called before the first frame update
    void Start()
    {
        distanceFromCamera = Vector3.Distance(OculusCamera.transform.position, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward_vector = OculusCamera.transform.forward;
        Vector3 origin_position = OculusCamera.transform.position;
        origin_position.y = origin_position.y - canvas_height;
        Vector3 resultingPosition = origin_position + forward_vector * distanceFromCamera;
        transform.position = Vector3.Lerp(transform.position, resultingPosition, follow_speed * Time.deltaTime);

        Quaternion rotation = OculusCamera.transform.rotation;
        transform.rotation = rotation;
    }
}
