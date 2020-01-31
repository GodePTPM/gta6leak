using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour {
	private Animator anim;
	public float playerSpeed = 10f;
	float newPlayerSpeed;
	public float gravity = 20f;
	CharacterController characterController;
	public GameObject playerHead;
	public GameObject playerArmature;
	Vector3 lastVelocity;
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
		anim.SetBool("pedArmed", true);
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
		transform.Translate(new Vector3(0,0.7f,-0.55f));
		characterController.enabled = true;
		playerTask = "IDLE";
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
        	anim.SetBool("LAND", false);
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
		RaycastHit hit;
		resetTasks();
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
		if (Input.GetKeyDown("c") && canJump && characterController.isGrounded && playerTask != "SPRINT" && playerTask != "RUN" && controlsEnabled && playerTask != "LAND" && playerTask != "JUMPLAND" && canGetup) {
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
		}
		Vector3 moveDirection = characterController.velocity;
		if (playerTask != "JUMP" && playerTask != "GLIDE" && prevPlayerTask != "GLIDE" && playerTask != "LAND" && playerTask != "LAND") {
			playerTask = "N/A";
		}
		Vector3 camPos = Camera.main.transform.rotation.eulerAngles;
		float verticalAxis = Input.GetAxisRaw("Vertical");
		float horizontalAxis = Input.GetAxisRaw("Horizontal");
		if (characterController.isGrounded && controlsEnabled) {
			if (playerTask == "JUMP") {
				canJump = false;
				StartCoroutine(enableJump());
			}
			moveDirection = new Vector3(0,0,0);
			if  (verticalAxis == -1 && horizontalAxis == 1) {
				float angle = Mathf.LerpAngle(transform.eulerAngles.y, camPos.y-45, 0.3f);
				transform.eulerAngles = new Vector3(0, angle, 0);
			} else if (verticalAxis == 1 && horizontalAxis == 1) {
				float angle = Mathf.LerpAngle(transform.eulerAngles.y, camPos.y+225, 0.3f);
				transform.eulerAngles = new Vector3(0, angle, 0);
			} else if (verticalAxis == 1 && horizontalAxis == -1) {
				float angle = Mathf.LerpAngle(transform.eulerAngles.y, camPos.y+135, 0.3f);
				transform.eulerAngles = new Vector3(0, angle, 0);
			} else if (verticalAxis == -1 && horizontalAxis == -1) {
				float angle = Mathf.LerpAngle(transform.eulerAngles.y, camPos.y+45, 0.3f);
				transform.eulerAngles = new Vector3(0, angle, 0);
			} else if (verticalAxis == 1) {
				float angle = Mathf.LerpAngle(transform.eulerAngles.y, camPos.y-180, 0.3f);
				transform.eulerAngles = new Vector3(0, angle, 0);
			} else if (horizontalAxis == 1) {
				float angle = Mathf.LerpAngle(transform.eulerAngles.y, camPos.y-90, 0.3f);
				transform.eulerAngles = new Vector3(0, angle, 0);
			} else if (horizontalAxis == -1) {
				float angle = Mathf.LerpAngle(transform.eulerAngles.y, camPos.y+90, 0.3f);
				transform.eulerAngles = new Vector3(0, angle, 0);
			} else if  (verticalAxis == -1) {
				float angle = Mathf.LerpAngle(transform.eulerAngles.y, camPos.y, 0.3f);
				transform.eulerAngles = new Vector3(0, angle, 0);
			} else {
				playerTask = "IDLE";
				newPlayerSpeed = 0f;
			}
			if (playerTask != "IDLE") {
				if (Input.GetButton("Sprint") && canGetup) {
					moveDirection = -transform.forward*newPlayerSpeed;
					if (pedDucked) {
						pedDucked = false;
					}
					playerTask = "SPRINT";
				} else {
					if (pedDucked == false) {
						moveDirection = -transform.forward*(newPlayerSpeed*0.6f);
					} else {
						moveDirection = -transform.forward*(newPlayerSpeed*0.4f);
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

		if ((playerTask == "IDLE" || playerTask == "RUN" || playerTask == "SPRINT") && characterController.isGrounded && playerTask != "JUMP" && playerTask != "GRAB") {
			moveDirection.y -= (gravity*10) * Time.deltaTime;
		} else {
			moveDirection.y -= gravity * Time.deltaTime;
		}

		characterController.Move(moveDirection * Time.deltaTime);

		Vector3 grabPosition1;
		Vector3 grabPosition2;
		Vector3 grabPrevPosition1;
		Vector3 grabPrevPosition2;
		RaycastHit hitUp;
		RaycastHit hitDown;

		if (!anim.GetBool("pedAiming")) {
			for (int i=1; i <= 10; i++) {
				grabPosition1 = new Vector3(transform.position.x,(transform.position.y-0.125f)+(0.1f*i),transform.position.z);
				grabPosition2 = grabPosition1+(-transform.forward*0.4f);
				grabPrevPosition1 = new Vector3(transform.position.x,(transform.position.y-0.125f)+(0.1f*(i-1)),transform.position.z);
				grabPrevPosition2 = grabPrevPosition1+(-transform.forward*0.4f);
				if(!Physics.Linecast(grabPosition1,grabPosition2,out hitUp) && Physics.Linecast(grabPrevPosition1,grabPrevPosition2,out hitDown) && playerTask == "JUMP") {
					playerTask = "GRAB";
					characterController.enabled = false;
					transform.position = new Vector3(transform.position.x,grabPosition1.y-(0.1f),transform.position.z);
					characterController.enabled = true;
					StartCoroutine(backNormalGrab(0.8f));
				}
			}
		}
		if (prevPlayerTask == "GLIDE" && playerTask != "GLIDE" && playerTask == "IDLE") {
			playerTask = "LAND";
		}
		if (prevPlayerTask == "GLIDE" && playerTask != "GLIDE" && (playerTask == "SPRINT" || playerTask == "RUN")) {
			playerTask = "JUMPLAND";
		}
		if (prevPlayerTask == "GRAB" && playerTask != "GRAB") {
			playerTask = "IDLE";
		}
		if (prevPlayerTask == "JUMP" && playerTask != "JUMP" && playerTask != "GRAB" && playerTask == "IDLE") {
			playerTask = "LAND";
		}
		if (prevPlayerTask == "JUMP" && playerTask != "JUMP" && playerTask != "GRAB" && (playerTask == "SPRINT" || playerTask == "RUN")) {
			playerTask = "JUMPLAND";
		}
		if (prevPlayerTask == "JUMPLAND" && playerTask != "JUMPLAND") {
			playerTask = "JUMPLAND";
		}
		if (playerTask == "N/A") {
			playerTask = prevPlayerTask;
		}
		anim.SetBool(playerTask, true);
		if (anim.GetBool("pedDucked") != pedDucked) {
			fixControllerHeight();
		}
		anim.SetBool("pedDucked", pedDucked);
		prevPlayerTask = playerTask;
	}
	
	 /* void LateUpdate() {
		Vector3 camPos = Camera.main.transform.rotation.eulerAngles;
		Quaternion lookPosition = Quaternion.Euler(0, camPos.y-90, -camPos.x+180);
		lookPosition.eulerAngles = new Vector3(lookPosition.eulerAngles.x, lookPosition.eulerAngles.y, lookPosition.eulerAngles.z);
		playerHead.transform.rotation = lookPosition;
	} */

	public float pushPower = 2.0F;

	void OnControllerColliderHit(ControllerColliderHit hit) {
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic) {
			return;
		}
		if (hit.moveDirection.y < -0.3f) {
			return;
		}
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		body.velocity = pushDir * pushPower;
	}
}