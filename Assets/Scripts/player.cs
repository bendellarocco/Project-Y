﻿using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

	public float speed = 20;
	public float jumpVelocity = 10;
	public float grappleVelocity = 20;
	public LayerMask playerMask;
	Transform myTrans, tagGround;
	Rigidbody2D mybody;
	Vector3 target;
	float swingDirection;
	
	public bool isGrounded = false;

	// Use this for initialization
	void Start () {
		mybody = this.GetComponent<Rigidbody2D>();
		myTrans = this.transform;
		tagGround = GameObject.Find(this.name + "/tag_ground").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		target = transform.position;
		isGrounded = Physics2D.Linecast (myTrans.position, tagGround.position, playerMask);

		Move (Input.GetAxisRaw ("Horizontal"));
		if (Input.GetButtonDown ("Jump")) {
			Jump ();
		}

	}
		
	public void Move(float horizontalInput){
		Vector2 moveVelocity = mybody.velocity;
		moveVelocity.x = horizontalInput * speed;
		mybody.velocity = moveVelocity;
	}

	public void Jump() {
		if (isGrounded == true) {
			mybody.velocity += jumpVelocity * Vector2.up;
		}

	}
}