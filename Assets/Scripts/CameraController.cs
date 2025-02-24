using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float speed = 20;
	private Vector3 motion;
	public Camera cam;
	private float zoomSpeed = 1600;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        motion = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
		transform.Translate(motion * speed * Time.deltaTime);
        float nextSize = cam.orthographicSize -= Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        if (nextSize >= 1 && nextSize < 150)
            cam.orthographicSize = nextSize;
    }
}
