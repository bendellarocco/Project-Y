using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

	float speed = 60;
	float jumpVelocity = 7;
	float swingVelocity = 3;
	public static Transform myTrans; 
	Transform tagGround;
	public static Rigidbody2D mybody;
	bool isGrounded = false;
	public static bool wallJumped = false;
	int mask = 1 << 9;
	float rotation;
	public static GameObject touching;

	
	void Start () {
		mask = ~mask;
		mybody = this.GetComponent<Rigidbody2D>();
		myTrans = this.transform;
		tagGround = GameObject.Find(this.name + "/tag_ground").transform;
		rotation = transform.localScale.x;
	}

	void FixedUpdate () {

		isGrounded = Physics2D.Linecast (myTrans.position, tagGround.position, mask);

		//I HATE THIS BUT ILL FIX IT LATER
		if (isGrounded == true) {
			stickyWall.wallStuck = false;
		}

		//TEST IF YOURE FALLING TOO FAST YOU DIE
		if (mybody.velocity.y < -25) {
			Application.LoadLevel (Application.loadedLevel);;
			Debug.Log("DIEDFROMFALLING");
		}

		//MOVE/SWING
		//OLD WAY Input.acceleration.x > .09 || Input.acceleration.x < -.09
		if (Input.acceleration.x != 0) {
			if (stickyWall.wallStuck == false) {
				if (Input.acceleration.x > .001) {
					transform.localScale = new Vector3 (rotation, transform.localScale.y, transform.localScale.z);
				} else {
					transform.localScale = new Vector3 (-rotation, transform.localScale.y, transform.localScale.z);
				}

				if (grappleHook.isHooked == false || grappleHook.interacting == true) {
					Move ((Input.acceleration), speed);
				} else {
					Swing ((Input.acceleration), swingVelocity);
				}
			}
		}
	}
		
	public void Move(Vector2 horizontalInput, float momentum){
		//NORMALIZE INPUT SO ITS LESS TWITCHY
		horizontalInput.Normalize();

		//MULTIPLY BY DELTATIME SO IT MOVES PER SECOND NOT PER FRAME
		horizontalInput *= Time.deltaTime;
		horizontalInput *= momentum;
		//horizontalInput.x = Mathf.Abs (horizontalInput.x);


		//SLOW DOWN INPUT IF ITS TOO HIGH
		if (horizontalInput.x > .15) {
			horizontalInput.x = .15f;
		} else 
			if (horizontalInput.x < -.15) {
			horizontalInput.x = -.15f;
		}

		//MOVE PLAYER
		transform.Translate((horizontalInput.x), 0, 0);
	}

	public void Swing(Vector2 horizontalInput, float momentum){
		//horizontalInput.x = Mathf.Abs (horizontalInput.x);
			mybody.AddForce(transform.right * horizontalInput.x * momentum);
	}

	public void Jump() {
		//CORRECT JUMP ALGORITH.  CHECKS IF PLAYER IS ON THE GROUND/HOOKED/OR STUCK TO WALL
		if (isGrounded == true && grappleHook.isHooked == false) {
			mybody.velocity += jumpVelocity * Vector2.up;
		} else {
			if (stickyWall.wallStuck == true) {
				if (wallJumped == false) {
					mybody.drag = 0;
					wallJumped = true;
					wallJump ();
				}
			}
		}
	}

	public void wallJump() {
		//Jumping if youre on the wall
		if (touching.name == "Right Stick") {
			transform.localScale = new Vector3 (rotation, transform.localScale.y, transform.localScale.z);
			mybody.AddForce (myTrans.right * 100);
		} else {
			transform.localScale = new Vector3 (-rotation, transform.localScale.y, transform.localScale.z);
			mybody.AddForce (-myTrans.right * 100);
		}
		mybody.velocity += jumpVelocity * Vector2.up;
	}

	public static void Release() {
		Destroy (mybody.GetComponent<SpringJoint2D>(), 0);
		Destroy (mybody.GetComponent<DistanceJoint2D>(), 0);
		grappleHook.isHooked = false;
		grappleHook.interacting = false;
	}
}