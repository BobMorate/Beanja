using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	private static float smallDistance = 0.2f;

	public LayerMask GroundMask;
	public LayerMask PlattformMask;

	public float walkSpeed;
	public float turnAroundWaitTime;
	public enum WalkDirection { left = -1, stand = 0, right = 1}
	public WalkDirection startWalkingDirection;

	private Vector2 startPosition;
	private WalkDirection currentWalkingDirection;
	private float turnAroundWaitTimer = 0;
	private WalkDirection turnTowardsWalkDirection;

	void Start () {
		transform.position = Physics2D.Raycast(transform.position, -Vector2.up, float.PositiveInfinity, GroundMask).point;
		startPosition = transform.position;
		currentWalkingDirection = startWalkingDirection;
	}
	
	void FixedUpdate () {
		UpdateAI();
		UpdateAnimation();
	}

	private void UpdateAI()
	{
		Vector2 vel = rigidbody2D.velocity;
		Vector2 pos = transform.position;
		Bounds colliderBounds = collider2D.bounds;

		if(turnAroundWaitTimer > 0)
		{
			UpdateWaiting();
		}
		if(currentWalkingDirection != WalkDirection.stand)
		{
			UpdateWalking(ref pos, ref colliderBounds);
		}
		vel.x = walkSpeed * (int)currentWalkingDirection;
		rigidbody2D.velocity = vel;
	}

	private void UpdateWaiting()
	{
		turnAroundWaitTimer -= Time.deltaTime;
		if (turnAroundWaitTimer <= 0)
		{
			currentWalkingDirection = turnTowardsWalkDirection;
		}
	}

	private void UpdateWalking(ref Vector2 pos, ref Bounds colliderBounds)
	{
		Vector2 checkPosition = pos;
		if (currentWalkingDirection == WalkDirection.left)
			checkPosition = new Vector2(colliderBounds.center.x - colliderBounds.extents.x, colliderBounds.center.y - colliderBounds.extents.y);
		if (currentWalkingDirection == WalkDirection.right)
			checkPosition = new Vector2(colliderBounds.center.x + colliderBounds.extents.x, colliderBounds.center.y - colliderBounds.extents.y);
		if (Physics2D.Raycast(checkPosition, -Vector2.up, smallDistance, GroundMask).collider == null ||
			Physics2D.Raycast(checkPosition + Vector2.up * smallDistance, Vector2.up, colliderBounds.extents.y * 2 - smallDistance, GroundMask).collider != null)
		{
			turnAroundWaitTimer = turnAroundWaitTime;
			turnTowardsWalkDirection = (WalkDirection)((int)currentWalkingDirection * -1);
			currentWalkingDirection = WalkDirection.stand;
		}
	}

	private void UpdateAnimation()
	{
		Vector2 vel = rigidbody2D.velocity;
		if(Mathf.Abs(vel.x) > 0.1f)
		{
			transform.localScale = new Vector3(vel.x > 0 ? 1 : -1,1,1);
		}
		
		if(Mathf.Abs(vel.x) > 0.1f)
		{
			//spriteAnimator.Play("Walk");
		}
		else
		{
			//spriteAnimator.Play("Idle");
		}
		
	}
}
