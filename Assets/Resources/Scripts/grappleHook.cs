﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class grappleHook : MonoBehaviour {

	public GameObject playerGo;
	public Transform jointTransform;
	public Rigidbody2D joint;
	Vector3 target;
	float grappleDistance;
	public LineRenderer lineRenderer;
	public static bool isHooked = false;
	private DistanceJoint2D distanceJoint;
	float maxGrapple;
	
	void Start () {
		lineRenderer = this.GetComponent<LineRenderer>();
	}

	void Update () {

		//REMOVE LINE IF NOT HOOKED
		if (isHooked == false) {
			lineRenderer.SetPosition (1, this.transform.position);
		} else {
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

				RaycastHit2D swingCollide = Physics2D.Linecast (start, end);
				if (swingCollide.collider != null) {
				player.Release();
				isHooked = false;
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
				RaycastHit2D hit = Physics2D.Linecast (transform.position, target);
				if (hit.collider != null) {

					//TEST IF GRAPPLE IS OVER MAX ALLOWED DISTANCE
					maxGrapple = target.x - playerGo.transform.position.x;
					grappleDistance = target.y - playerGo.transform.position.y;


					if (maxGrapple < 15 && maxGrapple > -15) {
						//IF COLLISION DRAW LINE FROM PLAYER TO HIT LOCATION
						lineRenderer.SetPosition (1, hit.point);

						//DETERMINE IF GRABBLE DISTANCE IS A CEILING
						if (hit.collider.tag == "sticky_wall") {
							grappleDistance = 1;
						} else {
							grappleDistance = (grappleDistance - 2);
						}

						//MOVE JOINT
						jointTransform.position = new Vector3 (hit.point.x, hit.point.y, 0);

						//CREATE DISTANCE JOINT
						distanceJoint = playerGo.AddComponent<DistanceJoint2D> ();
						distanceJoint.distance = grappleDistance;
						distanceJoint.connectedBody = joint;

						isHooked = true;

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
