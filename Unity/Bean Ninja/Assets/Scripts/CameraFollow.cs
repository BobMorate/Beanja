using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public Transform target;
    public float followSpeed = 1;
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
        pos.z = transform.position.z;
        transform.position = pos;
	}
}
