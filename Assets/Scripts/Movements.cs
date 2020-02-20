using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour {
	private Animator anim;
	public float playerSpeed = 10f;
	public float gravity = 20f;

	float newPlayerSpeed;
	CharacterController characterController;
	string playerTask = "N/A";
	string prevPlayerTask = "N/A";
	float distance = 0f;
	bool canJump = true;
	bool pedDucked = false;
	bool controlsEnabled = true;
	bool canGetup = false;

	void Start() {
		characterController = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		anim = gameObject.GetComponent<Animator>();
		anim.SetBool("pedArmed", false);
	}

	public void jumpLanchEvent() {
		resetTasks();
		playerTask = "N/A";
		prevPlayerTask = "N/A";
		anim.SetBool(playerTask, true);
	}

	IEnumerator enableJump() {
		yield return new WaitForSeconds(0.4f);
		canJump = true;
	}

	IEnumerator backNormalGrab(float backTime) {
		yield return new WaitForSeconds(backTime);
		characterController.enabled = false;
		transform.position += ((-transform.forward*0.5f)+transform.up);
		characterController.enabled = true;
		playerTask = "IDLE";
		prevPlayerTask = "IDLE";
		newPlayerSpeed = 0f;
		anim.SetBool(playerTask, true);
	}

	bool backNormal_Active = false;

	IEnumerator backNormal(float backTime) {
		backNormal_Active = true;
		yield return new WaitForSeconds(backTime);
		backNormal_Active = false;
		playerTask = "N/A";
		newPlayerSpeed = 0f;
		anim.SetBool(playerTask, true);
	}

	bool backControls_Active = false;

	IEnumerator backControls(float backTime) {
		backControls_Active = true;
		yield return new WaitForSeconds(backTime);
		backControls_Active = false;
		controlsEnabled = true;
	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 500, 20), "Press 'R' to respawn.");
	}

	void resetTasks() {
		anim.SetBool("LAND", false);
		anim.SetBool("GRAB", false);
		anim.SetBool("SPRINT", false);
		anim.SetBool("RUN", false);
		anim.SetBool("IDLE", false);
		anim.SetBool("JUMP", false);
		anim.SetBool("JUMPLAND", false);
		anim.SetBool("GLIDE", false);
	}

	void fixControllerHeight() {
		if (!pedDucked) {
			characterController.enabled = false;
			characterController.height = 1.75f;
			characterController.center = new Vector3(0, 0, 0);
			characterController.enabled = true;
		} else {
			characterController.enabled = false;
			characterController.height = 1.28f;
			characterController.center = new Vector3(0, -0.2f, 0);
			characterController.enabled = true;
		}
	}

	void LateUpdate() {
		if (!Physics.Linecast(transform.position, transform.position+transform.up)) {
			canGetup = true;
		} else {
			canGetup = false;
		}
	}

	void Update() {
		Camera.main.GetComponent<SmoothFollow>().target = transform;
		resetTasks();
		RaycastHit hit;
		anim.SetBool(playerTask, true);
		if (Input.GetMouseButton(1)) {
			anim.SetBool("pedAiming", true);
		} else {
			anim.SetBool("pedAiming", false);
		}
		if (Input.GetKeyDown("r")) {
			characterController.enabled = false;
			transform.position = new Vector3(14, 0, 8);
			characterController.enabled = true;
			return;
		}

		if (Input.GetKeyDown("c") && canJump && playerTask == "IDLE" && controlsEnabled && canGetup) {
			characterController.Move(new Vector3(0,0,0));
			pedDucked = !pedDucked;
			playerTask = "IDLE";
			anim.SetBool(playerTask, true);
			anim.SetBool("pedDucked", pedDucked);
			fixControllerHeight();
		}
		if (playerTask == "LAND" && !backNormal_Active && !backNormal_Active && !backControls_Active) {
			controlsEnabled = false;
			StartCoroutine(backNormal(0.5f));
			StartCoroutine(backControls(0.5f));
			characterController.Move(new Vector3(0,0,0));
		}
		if (playerTask == "GRAB") {
			characterController.Move(new Vector3(0,0,0));
			return;
		}
		var targetPos = transform.position;
		targetPos.y -= 10;
		if(Physics.Linecast(transform.position, targetPos, out hit)) {
			distance = hit.distance;
		} else {
			distance = 100;
		}
		Vector3 moveDirection = characterController.velocity;
		if (playerTask != "JUMP" && playerTask != "LAND" && playerTask != "JUMPLAND" && playerTask != "GRAB") {
			playerTask = "N/A";
		}
		var camTransform = Camera.main.transform;
		if (characterController.isGrounded && controlsEnabled) {
			if (playerTask == "JUMP") {
				canJump = false;
				StartCoroutine(enableJump());
			}
			moveDirection = new Vector3(0,0,0);
			if  (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
				var targetMove = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
				moveDirection = camTransform.TransformDirection(targetMove);
				var targetRotation = Quaternion.LookRotation(-moveDirection,Vector3.up);
				float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, 0.3f);
				transform.eulerAngles = new Vector3(0,angle,0);
			} else {
				playerTask = "IDLE";
				newPlayerSpeed = 0f;
			}
			if (playerTask != "IDLE") {
				if (Input.GetButton("Sprint") && canGetup) {
					moveDirection = moveDirection.normalized*newPlayerSpeed;
					if (pedDucked) {
						pedDucked = false;
					}
					playerTask = "SPRINT";
				} else {
					if (pedDucked == false) {
						moveDirection = moveDirection.normalized*(newPlayerSpeed*0.6f);
					} else {
						moveDirection = moveDirection.normalized*(newPlayerSpeed*0.4f);
					}
					playerTask = "RUN";
				}
				if (playerTask == "RUN" || playerTask == "SPRINT") {
					if (newPlayerSpeed < playerSpeed) {
						newPlayerSpeed = newPlayerSpeed+0.2f;
					}
				}
			}
			if (Input.GetButton("Jump") && canJump == true && canGetup) {
				moveDirection.y = 7.0f;
				playerTask = "JUMP";
				pedDucked = false;
			}
		}
		if (playerTask == "N/A" && distance > 1.6f) {
			playerTask = "GLIDE";
			pedDucked = false;
		}
		if ((playerTask == "IDLE" || playerTask == "RUN" || playerTask == "SPRINT") && characterController.isGrounded) {
			moveDirection.y -= (gravity*10) * Time.deltaTime;
		} else {
			moveDirection.y -= gravity * Time.deltaTime;
		}
		if ((prevPlayerTask == "JUMP" || prevPlayerTask == "GLIDE") && playerTask == "IDLE") {
			playerTask = "LAND";
			canJump = false;
			StartCoroutine(enableJump());
		}
		if ((prevPlayerTask == "JUMP" || prevPlayerTask == "GLIDE") && (playerTask == "RUN" || playerTask == "SPRINT")) {
			playerTask = "JUMPLAND";
			canJump = false;
			StartCoroutine(enableJump());
		}

		characterController.Move(moveDirection * Time.deltaTime);

		RaycastHit hitUp;
		RaycastHit hitDown;
		for (int i=1; i <= 10; i++) {
			if (playerTask != "JUMP") { break; }
			var grabPosition1 = new Vector3(transform.position.x,(transform.position.y-0.125f)+(0.1f*i),transform.position.z);
			var grabPosition2 = grabPosition1+(-transform.forward*0.4f);
			var grabPrevPosition1 = new Vector3(transform.position.x,(transform.position.y-0.125f)+(0.1f*(i-1)),transform.position.z);
			var grabPrevPosition2 = grabPrevPosition1+(-transform.forward*0.4f);
			if(!Physics.Linecast(grabPosition1,grabPosition2,out hitUp) && Physics.Linecast(grabPrevPosition1,grabPrevPosition2,out hitDown)) {
				playerTask = "GRAB";
				characterController.enabled = false;
				transform.position = grabPosition1-new Vector3(0,0.1f,0);
				characterController.enabled = true;
				StartCoroutine(backNormalGrab(0.8f));
			}
		}
		if (playerTask == "N/A") {
			playerTask = prevPlayerTask;
		}
		if (anim.GetBool("pedDucked") != pedDucked) {
			fixControllerHeight();
		}
		anim.SetBool(playerTask, true);
		anim.SetBool("pedDucked", pedDucked);
		prevPlayerTask = playerTask;
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic) {
			return;
		}
		if (hit.moveDirection.y < -0.3f) {
			return;
		}
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		body.velocity = pushDir * 2.0F;
	}
}