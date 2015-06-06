﻿using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

	public float speed = 7;
	public float jumpVelocity = 7;
	public float releaseVelocity = 1;
	Transform myTrans, tagGround;
	Rigidbody2D mybody;
	public GameObject playerRid;
	public bool isGrounded = false;

	// Use this for initialization
	void Start () {
		mybody = this.GetComponent<Rigidbody2D>();
		myTrans = this.transform;
		tagGround = GameObject.Find(this.name + "/tag_ground").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		isGrounded = Physics2D.Linecast (myTrans.position, tagGround.position);

		//MOVE/SWING
		if (Input.acceleration.x > .09 || Input.acceleration.x < -.09) {
				if (grappleHook.isHooked == false) {
					Move ((Input.acceleration), speed);
				}else {
					Swing ((Input.acceleration), speed);
				}

		}


		//JUMP KEYS DOWN
		if (Input.GetButtonDown ("Jump")) {
			Jump ();
		}

	}
		
	public void Move(Vector2 horizontalInput, float momentum){
			//NORMALIZE INPUT SO ITS LESS TWITCHY
			horizontalInput.Normalize();

			//MULTIPLY BY DELTATIME SO IT MOVES PER SECOND NOT PER FRAME
			horizontalInput *= Time.deltaTime;
			transform.Translate((horizontalInput.x * 12), 0, 0);
	
	}

	public void Swing(Vector2 horizontalInput, float momentum){
			mybody.AddForce(transform.right * horizontalInput.x * 2);
	}

	public void Jump() {
		if ((isGrounded == true  || stickyWall.wallStuck == true) && grappleHook.isHooked == false) {
			mybody.drag = 0;
			mybody.velocity += jumpVelocity * Vector2.up;
		}else{
			if (grappleHook.isHooked == true) {
				Release ();
			}
		}
	}

	public void Release() {
		Destroy (playerRid.GetComponent<DistanceJoint2D>(), 0);
		mybody.velocity += releaseVelocity * Vector2.up;
		grappleHook.isHooked = false;
	}
}