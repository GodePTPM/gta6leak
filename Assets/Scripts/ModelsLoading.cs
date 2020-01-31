using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelsLoading : MonoBehaviour {
	public Component playerCharacter;
	public Component backpackObject;
	public Component helmetObject;
	public Component theWeapon;

	public float rX = 0;
	public float rY = 0;
	public float rZ = 0;

	public float posX = 0;
	public float posY = 0;
	public float posZ = 0;

	void Start() {
	}


	/* 

	array = {
		["FN 30-11"] = {355.73, 0, 0.1, -0.01, -0.14, 0.05},
	}
	
	headElement.transform.TransformPoint(posX, posY, posZ);

	void Update() {
		var theBone = playerCharacter.transform.Find("player_Armature").Find("Root").Find("Pelvis").Find("Spine_1").Find("Spine_2").Find("Neck").Find("Bip01_Clavicle_R").Find("UpperArm_R").Find("ForeArm_R").Find("Hand_R");
		theWeapon.transform.position = theBone.transform.position;
		theWeapon.transform.rotation = theBone.transform.rotation;
		theWeapon.transform.localPosition = new Vector3(posX,posY,posZ);
		theWeapon.transform.parent = theBone.transform;
	}

	*/

	void LateUpdate() {

		var theBone = playerCharacter.transform.Find("player_Armature").Find("Root").Find("Pelvis").Find("Spine_1").Find("Spine_2").Find("Neck").Find("Bip01_Clavicle_R").Find("UpperArm_R").Find("ForeArm_R").Find("Hand_R");
		theWeapon.transform.position = theBone.transform.TransformPoint(-0.01f, -0.14f, 0.05f);
		theWeapon.transform.rotation = theBone.transform.rotation;
		theWeapon.transform.parent = theBone.transform;


		var backpack = playerCharacter.transform.Find("backpack");
		var helmet = playerCharacter.transform.Find("helmet");
        	if (Input.GetKeyDown("v")) {
			backpack.gameObject.SetActive(!backpack.gameObject.activeSelf);
			if (!backpack.gameObject.activeSelf) {
				var rotationBackpack = playerCharacter.transform.Find("player_Armature").Find("Root").transform.rotation;
				rotationBackpack *= Quaternion.Euler(0, 0, 180);
				var positionBackpack = playerCharacter.transform.Find("player_Armature").Find("Root").transform.position;
				var backpackCreated = Instantiate(backpackObject, positionBackpack, rotationBackpack);
				backpackCreated.gameObject.SetActive(true);
				Destroy(backpackCreated.gameObject, 10f);
				/* Physics.IgnoreCollision(backpackCreated.gameObject.GetComponent<Collider>(), playerCharacter.gameObject.GetComponent<Collider>()); */
			}
		}
		if (Input.GetKeyDown("b")) {
			helmet.gameObject.SetActive(!helmet.gameObject.activeSelf);
			if (!helmet.gameObject.activeSelf) {
				var headElement = playerCharacter.transform.Find("player_Armature").Find("Root").Find("Pelvis").Find("Spine_1").Find("Spine_2").Find("Neck").Find("Head");
				var rotationHelmet = headElement.transform.rotation;
				rotationHelmet *= Quaternion.Euler(90, 270, 0);
				var positionHelmet = headElement.transform.TransformPoint(-0.01f, 0.66f, 0.36f);
				var helmetCreated = Instantiate(helmetObject, positionHelmet, rotationHelmet);
				helmetCreated.gameObject.SetActive(true);
				Destroy(helmetCreated.gameObject, 10f);
				/* Physics.IgnoreCollision(helmetCreated.gameObject.GetComponent<Collider>(), playerCharacter.gameObject.GetComponent<Collider>()); */
			}
		}
	}
}
