using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class PlattformCollider : MonoBehaviour {
	public Rigidbody2D controllingRigidBody2D;
	// Use this for initialization
	void Start () {
		if(controllingRigidBody2D == null)
		{
			controllingRigidBody2D = GetComponentInParent<Rigidbody2D>();
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		collider2D.enabled = controllingRigidBody2D.velocity.y <= 0;
	}
}
