using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour {
	private Animator anim;
	public float playerSpeed = 10f;
	public float gravity = 20f;
	CharacterController characterController;
	float newPlayerSpeed;
	string playerTask = "N/A";

	void Start() {
		characterController = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		anim = gameObject.GetComponent<Animator>();
	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 500, 20), "Press 'R' to respawn.");
	}

	void resetTasks() {
		anim.SetBool("IDLE", false);
		anim.SetBool("SPRINT", false);
		anim.SetBool("RUN", false);
		anim.SetBool("N/A", false);
	}

	void Update() {
		Camera.main.GetComponent<SmoothFollow>().target = transform;
		playerTask = "N/A";
		resetTasks();
		if (Input.GetKeyDown("r")) {
			characterController.enabled = false;
			transform.position = new Vector3(14, 0, 8);
			characterController.enabled = true;
			return;
		}

		Vector3 moveDirection = characterController.velocity;
		var camTransform = Camera.main.transform;
		if (characterController.isGrounded) {
			moveDirection = new Vector3(0,0,0);
			if  (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
				var targetMove = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
				moveDirection = camTransform.TransformDirection(targetMove);
				var targetRotation = Quaternion.LookRotation(moveDirection,Vector3.up);
				float angle = Mathf.LerpAngle(transform.eulerAngles.y, camTransform.eulerAngles.y, 0.3f);
				transform.eulerAngles = new Vector3(0,angle,0);
			} else {
				playerTask = "IDLE";
			}
			if (playerTask != "IDLE") {
				moveDirection.y = 0.0f;
				moveDirection = moveDirection.normalized*playerSpeed;
				playerTask = "RUN";
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		characterController.Move(moveDirection * Time.deltaTime);
		anim.SetBool(playerTask, true);
	}
}