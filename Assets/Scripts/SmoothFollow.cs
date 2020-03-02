using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {
	public Transform target;
	public float distance = 6.0f;

	float xRotation = 5.0f;
	float yRotation = 5.0f;

	void LateUpdate() {
		if (!target) { return; }

		var lookPosition = target.position+new Vector3(0,10,0);
		transform.position = lookPosition;

        	xRotation += Input.GetAxis("Mouse X")*2;
        	yRotation -= Input.GetAxis("Mouse Y")*2;
		yRotation = Mathf.Clamp(yRotation, -50, 85);
		var currentRotation = Quaternion.Euler(yRotation, xRotation, 0);
		RaycastHit hit;
		var targetPos = (transform.position-currentRotation * Vector3.forward * distance);
		var targetMargin = 0.1f;
		if(Physics.Linecast(transform.position, new Vector3(targetPos.x, targetPos.y-targetMargin, targetPos.z), out hit)) {
			targetPos = hit.point+(hit.normal*targetMargin);
		}
		var aimAddition = (target.transform.right*2f);
		transform.position = targetPos+aimAddition;
		transform.LookAt(lookPosition+aimAddition);

	}
}