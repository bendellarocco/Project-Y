﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class grappleHook : MonoBehaviour {

	public GameObject playerGo;
	public Transform jointTransform;
	public Rigidbody2D joint;
	Vector3 target;
	Vector3 center;
	float grappleDistance;
	float originalDistance;
	public LineRenderer lineRenderer;
	public static bool isHooked = false;
	public static bool interacting = false;
	private DistanceJoint2D distanceJoint;
	private DistanceJoint2D springJoint;
	float maxGrapple;
	int mask = 1 << 9;
	
	void Start () {
		lineRenderer = this.GetComponent<LineRenderer>();
		mask = ~mask;
	}

	void Update () {
		//REMOVE LINE IF NOT HOOKED
		if (isHooked == false) {
			lineRenderer.SetPosition (1, this.transform.position);
		} else {

			//PROPERLY PLACES JOINT
			Vector2 start;
			Vector2 end;
			start.x = transform.position.x;
			end.x = joint.position.x;
			start.y = transform.position.y;

			if (target.y > transform.position.y){
				end.y = joint.position.y - .5f;
				if(end.x > start.x){
					end.x = end.x - .1f;
				}else{
					end.x = end.x + .1f;
				}

			}else {
				end.y = target.y + .5f;
				if(end.x > start.x){
					end.x = end.x - .1f;
				}else{
					end.x = end.x + .1f;
				}
			}

				RaycastHit2D swingCollide = Physics2D.Linecast (start, end, mask);
				if (swingCollide.collider != null  && swingCollide.collider.tag != "interact_level") {
				player.Release();
				isHooked = false;
			}

			if (interacting == true){
				//IF YOU ARE CONNECTED TO AN INTERACTABLE OBJECT
				jointTransform.position = new Vector3(springJoint.connectedBody.position.x + (center.x / 2), springJoint.connectedBody.position.y+ (center.y / 2), 0);
				lineRenderer.SetPosition (1, joint.transform.position);
				float newDistance = Mathf.Round((playerGo.transform.position.x - springJoint.connectedBody.position.x));

				//IF YOU GO SHORTER YOU ARE DISSCONNECTED
				if(newDistance > originalDistance){
					player.Release();



				}
			}
		}

		//KEEP LINE ON PLAYER **THIS IS PROBABLY NOT THE BEST WAY TO DO THIS**
		lineRenderer.SetPosition (0, this.transform.position);

		if (Input.GetMouseButtonDown(0))
		{
			grapple();
		}
	}

	public void grapple(){

		if (isHooked == false) {

			Destroy (distanceJoint);
			//GET USERS MOUSE CLICK INPUT
			target = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			target.z = transform.position.z;

			//CHECKS IF CLICK IS OVER UI
			if (!EventSystem.current.IsPointerOverGameObject ()) {

				//SEND OUT RAY TO CHECK FOR ANY HITS
				RaycastHit2D hit = Physics2D.Linecast (transform.position, target, mask);
				if (hit.collider != null) {

					//TEST IF GRAPPLE IS OVER MAX ALLOWED DISTANCE
					maxGrapple = target.x - playerGo.transform.position.x;
					grappleDistance = target.y - playerGo.transform.position.y;

					if (maxGrapple < 15 && maxGrapple > -15) {
						//IF COLLISION DRAW LINE FROM PLAYER TO HIT LOCATION

						if (hit.collider.tag == "interact_level"){
							//IF COLLISION IS AN INTERACTABLE OBJECT
							interacting = true;
							springJoint = playerGo.AddComponent<DistanceJoint2D>();
							springJoint.connectedBody = hit.collider.attachedRigidbody;
							originalDistance = Mathf.Round((playerGo.transform.position.x - springJoint.connectedBody.position.x));

							if (target.x - playerGo.transform.position.x > 0) {
								springJoint.distance = target.x - playerGo.transform.position.x;
							}else {
								springJoint.distance = (target.x - playerGo.transform.position.x) * -1;
								originalDistance = originalDistance * -1;
							}
							center = hit.collider.bounds.size;
							springJoint.maxDistanceOnly = true;
							isHooked = true;
						}else {
							//DETERMINE IF GRABBLE DISTANCE IS A CEILING
							if (hit.collider.tag == "sticky_wall") {
								grappleDistance = 1;
							} else {
								grappleDistance = (grappleDistance - 2);
							}

							lineRenderer.SetPosition (1, hit.point);
							//MOVE JOINT
							jointTransform.position = new Vector3 (hit.point.x, hit.point.y, 0);

							//CREATE DISTANCE JOINT
							distanceJoint = playerGo.AddComponent<DistanceJoint2D> ();
							distanceJoint.distance = grappleDistance;
							distanceJoint.connectedBody = joint;

							isHooked = true;

						}
					} else {
						Debug.Log("TOO LONG");
					}
				} else {
					//NO HIT, REMOVE SECOND POINT OF LINE
					player.Release ();
					lineRenderer.SetPosition (1, this.transform.position);
				}

			}

		} else {
			player.Release ();
		}
	}
}
