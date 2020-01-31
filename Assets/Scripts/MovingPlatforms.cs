using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour {
	string targetName = "up";

	void changeTargetName() {
		if (targetName == "up") {
			targetName = "down";
			return;
		} else {
			targetName = "up";
			return;
		}
	}

	void Start() {
		InvokeRepeating("changeTargetName", 7, 10);
	}

	void FixedUpdate() {
		Vector3 targetPos = transform.position;
		if (targetName == "up") {
			targetPos.y += 1f;
			transform.position = Vector3.Lerp(transform.position, targetPos, 0.7f * Time.deltaTime);
		} else {
			targetPos.y += -1f;
			transform.position = Vector3.Lerp(transform.position, targetPos, 0.7f * Time.deltaTime);
		}
	}
}