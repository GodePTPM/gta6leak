using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {
	public Transform target;
	public float distance = 6.0f;

	float xRotation = 0.0f;
	float yRotation = 0.0f;

	void LateUpdate() {
		if (!target) { return; }

		Vector3 lookPosition = target.position+target.TransformDirection(5, 15, 1); // new Vector3(target.position.x, target.position.y+15, target.position.z);

        	xRotation += Input.GetAxis("Mouse X")*2;
        	yRotation -= Input.GetAxis("Mouse Y")*2;

		yRotation = Mathf.Clamp(yRotation, -50, 85);

		var currentRotation = Quaternion.Euler(yRotation, xRotation, 0);

		var targetPos = (lookPosition-currentRotation * Vector3.forward * distance);

		RaycastHit hit;

		if(Physics.Linecast(target.position+target.TransformDirection(0, 15, 0), new Vector3(targetPos.x, targetPos.y, targetPos.z), out hit)) {
			targetPos = hit.point+(hit.normal);
		}

		transform.position = targetPos;

		transform.rotation = currentRotation;


	//	// Rotation and Stuff


		float angle = Mathf.LerpAngle(target.eulerAngles.y, transform.eulerAngles.y, 0.3f);

		var player_spine = target.Find("Game_engine").Find("Root").Find("pelvis").Find("spine_01").Find("spine_02");
		player_spine.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y+10,0);

		target.eulerAngles = new Vector3(0,angle,0);
	}
}