using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour {
	private Animator anim;
	private float originalPlayerSpeed = 20f;
	private float playerSpeed = 0.0f;
	private float gravity = 250f;

	CharacterController characterController;
	string playerTask = "N/A";

	void Start() {
		characterController = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		anim = gameObject.GetComponent<Animator>();


		// var player_clavicleL = transform.Find("Game_engine").Find("Root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_l");
		// mp5Object = (GameObject)Instantiate(mp5Prefab, player_clavicleL.transform.position, Quaternion.identity);
		// mp5Object.transform.parent = player_clavicleL;
		// mp5Object.transform.localPosition = new Vector3(-0.14f, 0.01f, 0.38f);
	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 500, 20), "Press 'R' to respawn.");
	}

	void resetTasks() {
		anim.SetBool("IDLE", false);
		anim.SetBool("WALK", false);
		anim.SetBool("RUN", false);
		anim.SetBool("JUMP", false);

		anim.SetFloat("HorizontalAxis", 0.0F);
		anim.SetFloat("VerticalAxis", 0.0F);

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

		if (Input.GetMouseButton(0)) {
			anim.SetLayerWeight(2, 1);
		} else {
			anim.SetLayerWeight(2, 0);
		}

		Vector3 moveDirection = characterController.velocity;
		var camTransform = Camera.main.transform;
		if (characterController.isGrounded) {
			moveDirection = new Vector3(0,0,0);
			if  (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
				anim.SetFloat("HorizontalAxis", Input.GetAxisRaw("Horizontal"));
				anim.SetFloat("VerticalAxis", Input.GetAxisRaw("Vertical"));
				var targetMove = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
				moveDirection = transform.TransformDirection(targetMove);
				var targetRotation = Quaternion.LookRotation(moveDirection,Vector3.up);
			} else {
				playerTask = "IDLE";
				playerSpeed = 0.0f;
			}
			if (playerTask != "IDLE") {
				moveDirection.y = 0.0f;
				moveDirection = moveDirection.normalized*playerSpeed;
				
				playerSpeed = Mathf.Lerp(playerSpeed, originalPlayerSpeed, 0.05f);
				if (Input.GetKey(KeyCode.Space)) {
					playerTask = "RUN";
					originalPlayerSpeed = 50f;
				} else {
					playerTask = "WALK";
					originalPlayerSpeed = 20f;
				}
			}
			if (Input.GetKeyDown(KeyCode.LeftShift)) {
				moveDirection.y += 70;
				playerTask = "JUMP";
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		characterController.Move(moveDirection * Time.deltaTime);
		anim.SetBool(playerTask, true);
	}
}