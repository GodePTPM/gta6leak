using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {
	public Transform target;
	private Animator anim;
	public float distance = 6.0f;
	bool isAiming;
	public Texture crossHair;

	void Start() {
		if (!target) return;
		transform.position = target.position;
		anim = target.GetComponent<Animator>();
		isAiming = anim.GetBool("pedAiming");
	}

	float xRotation = 5.0f;
	float yRotation = 5.0f;

	void LateUpdate() {
		isAiming = anim.GetBool("pedAiming");
		transform.position = target.position;
        	xRotation += Input.GetAxis("Mouse X");
        	yRotation -= Input.GetAxis("Mouse Y");
		yRotation = Mathf.Clamp(yRotation, -50, 85);
		var currentRotation = Quaternion.Euler(yRotation, xRotation, 0);
		RaycastHit hit;
		var targetPos = transform.position-currentRotation * Vector3.forward * distance;
		if (isAiming) {
			targetPos = transform.position-currentRotation * Vector3.forward * 1;
		}
		var targetMargin = 0.1f;
		if(Physics.Linecast(transform.position, new Vector3(targetPos.x, targetPos.y-targetMargin, targetPos.z), out hit)) {
			targetPos = hit.point+(hit.normal*targetMargin);
		}
		if (isAiming) {
			var theBone = target.transform.Find("player_Armature").Find("Root").Find("Pelvis").Find("Spine_1").Find("Spine_2");
			var aimAddition = (-target.transform.right*0.5f)+(target.transform.up*0.7f);
			target.transform.eulerAngles = new Vector3(0,transform.eulerAngles.y-180,0);
			transform.position = targetPos+aimAddition;
			transform.LookAt(target.transform.position+aimAddition);
			theBone.rotation = transform.rotation;
			theBone.rotation *= Quaternion.Euler(180+10,145,0);
		} else {
			transform.position = targetPos;
			transform.LookAt(target);
		}

	}

	void OnGUI() {
		if (isAiming) {
			GUI.DrawTexture(new Rect(Screen.width*0.475f, Screen.height*0.475f, Screen.width*0.05f, Screen.height*0.05f), crossHair);
		}
	}
}