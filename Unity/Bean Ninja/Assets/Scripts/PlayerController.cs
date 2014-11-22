using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InputState))]
public class PlayerController : MonoBehaviour {
	private static float smallDistance = 0.2f;
	InputState inputState;
	tk2dSprite sprite;
	tk2dSpriteAnimator spriteAnimator;
	public Transform buttPos;
	public GameObject walkGasCloud;
	public GameObject jumpGasCloud;
	public GameObject wallJumpGasCloud;
	public GameObject doubleJumpGasCloud;
	public Transform cameraTarget;

	public float walkGasCloudRate;

	public bool isInAir { get { return fallDownReactionTimer <= 0; } }
	public int isSliding = 0;
	public LayerMask GroundMask;
	public LayerMask PlattformMask;

	public float walkSpeed;
	public float walkOnPlattformSpeed;
	public float maxJumpForce;
	public float breakJumpForce;
	public float airControl;
	public float fallDownReactionTime;
	public Vector2 jumpOffSlideSpeed;
	public float dropOffSlideSpeed;
	public float dropOffSlideTime;
	public float slideGrabStrength = 0;
	
	protected float  walkGasCloudTimer;
	protected float fallDownReactionTimer;
	protected float dropOffSlideTimer;

	protected bool jumpReleased;
	private float spawnCount = 1;
	private bool isBalancing = false;

	private bool doubleJumped = false;

	void Start () {
		CameraFollow follow = Camera.main.GetComponent<CameraFollow>();
		if(follow == null)
		{
			follow = Camera.main.gameObject.AddComponent<CameraFollow>();
		}
		follow.target = cameraTarget;
		inputState = GetComponent<InputState>();
		sprite = GetComponent<tk2dSprite>();
		spriteAnimator = GetComponent<tk2dSpriteAnimator>();
	}
	
	void FixedUpdate () {
		UpdateInput();
		UpdateAnimation();
	}

	private void UpdateInput()
	{
		Vector2 vel = rigidbody2D.velocity;
		Vector2 pos = transform.position;
		Bounds colliderBounds = collider2D.bounds;
		Vector2 upLeft = new Vector2(colliderBounds.center.x - colliderBounds.extents.x + smallDistance, colliderBounds.center.y - colliderBounds.extents.y + smallDistance);
		Vector2 bottomRight = new Vector2(colliderBounds.center.x + colliderBounds.extents.x - smallDistance, colliderBounds.center.y - colliderBounds.extents.y - smallDistance);
		if(vel.y <= 0 && Physics2D.OverlapArea(upLeft, bottomRight, GroundMask.value | PlattformMask.value) != null)
		{
			fallDownReactionTimer = fallDownReactionTime;
		}
		else
		{
			fallDownReactionTimer -= Time.deltaTime;
		}
		if(vel.y <= 0 && Physics2D.OverlapArea(upLeft, bottomRight, PlattformMask.value) != null)
		{
			isBalancing = true;
		}
		else
		{
			isBalancing = false;
		}



		if(!inputState.jumpDown)
		{
			jumpReleased = true;
		}
		if (!isInAir)
		{
			isSliding = 0;
			doubleJumped = false;
			if (inputState.leftDown)
			{
				vel.x = -1 * (isBalancing ? walkOnPlattformSpeed : walkSpeed);
			}
			else if (inputState.rightDown)
			{
				vel.x = isBalancing ? walkOnPlattformSpeed : walkSpeed;
			}
			else
			{
				vel.x = 0;
			}
			if (inputState.jumpDown && jumpReleased)
			{
				vel.y = maxJumpForce;
				fallDownReactionTimer = 0;
				jumpReleased = false;
				Instantiate(jumpGasCloud, buttPos.position, Quaternion.identity);
			}
		}
		else if(isSliding != 0)
		{
			doubleJumped = false;
			vel.x = 0;
			if(isSliding > 0)
			{
				if(inputState.rightDown)
				{
					dropOffSlideTimer -= Time.deltaTime;
					if(dropOffSlideTimer < 0)
					{
						vel.x = dropOffSlideSpeed;
						isSliding = 0;
						rigidbody2D.gravityScale = 1;
					}
				}
				else
				{
					dropOffSlideTimer = dropOffSlideTime;
				}
				if(inputState.leftDown)
				{
					vel.y = 0;
					rigidbody2D.gravityScale = 0;
				}
				else
				{ 
					rigidbody2D.gravityScale = 1;
				}

				// check left Slide
				upLeft = new Vector2(colliderBounds.center.x - colliderBounds.extents.x - smallDistance, colliderBounds.center.y + colliderBounds.extents.y - smallDistance);
				bottomRight = new Vector2(colliderBounds.center.x - colliderBounds.extents.x + smallDistance, colliderBounds.center.y - colliderBounds.extents.y + smallDistance);
				if(!Physics2D.OverlapArea(upLeft, bottomRight, GroundMask.value))
				{
					isSliding = 0;
					rigidbody2D.gravityScale = 1;
				}
			}
			if(isSliding < 0)
			{
				if(inputState.leftDown)
				{
					dropOffSlideTimer -= Time.deltaTime;
					if(dropOffSlideTimer < 0)
					{
						vel.x = -dropOffSlideSpeed;
						isSliding = 0;
						rigidbody2D.gravityScale = 1;
					}
				}
				else
				{
					dropOffSlideTimer = dropOffSlideTime;
				}
				if(inputState.rightDown)
				{
					vel.y = 0;
					rigidbody2D.gravityScale = 0;
				}
				else
				{ 
					rigidbody2D.gravityScale = 1;
				}

				// check right Slide
				upLeft = new Vector2(colliderBounds.center.x + colliderBounds.extents.x - smallDistance, colliderBounds.center.y + colliderBounds.extents.y - smallDistance);
				bottomRight = new Vector2(colliderBounds.center.x + colliderBounds.extents.x + smallDistance, colliderBounds.center.y - colliderBounds.extents.y + smallDistance);
				if(!Physics2D.OverlapArea(upLeft, bottomRight, GroundMask.value))
				{
					isSliding = 0;
					rigidbody2D.gravityScale = 1;
				}
			}
			if (inputState.jumpDown && jumpReleased)
			{
				vel = Vector2.Scale(jumpOffSlideSpeed, new Vector2(isSliding,1));
				fallDownReactionTimer = 0;
				isSliding = 0;
				jumpReleased = false;
				rigidbody2D.gravityScale = 1;
				Instantiate(wallJumpGasCloud, buttPos.position, Quaternion.identity);
			}
		}
		else
		{
			if (inputState.leftDown)
			{
				vel.x += -airControl * Time.deltaTime;
				vel.x = Mathf.Max(vel.x, -walkSpeed);
			}
			else if (inputState.rightDown)
			{
				vel.x += airControl * Time.deltaTime;
				vel.x = Mathf.Min(vel.x, walkSpeed);
			}
			if (!doubleJumped && inputState.jumpDown && jumpReleased)
			{
				vel.y = maxJumpForce;
				fallDownReactionTimer = 0;
				jumpReleased = false;
				Instantiate(doubleJumpGasCloud, buttPos.position, Quaternion.identity);
				doubleJumped = true;
			}
			// check right Slide
			upLeft = new Vector2(colliderBounds.center.x + colliderBounds.extents.x - smallDistance, colliderBounds.center.y + colliderBounds.extents.y - smallDistance);
			bottomRight = new Vector2(colliderBounds.center.x + colliderBounds.extents.x + smallDistance, colliderBounds.center.y - colliderBounds.extents.y + smallDistance);
			if(Physics2D.OverlapArea(upLeft, bottomRight, GroundMask.value))
			{
				isSliding = -1;
				dropOffSlideTimer = 0;
			}
			
			// check left Slide
			upLeft = new Vector2(colliderBounds.center.x - colliderBounds.extents.x - smallDistance, colliderBounds.center.y + colliderBounds.extents.y - smallDistance);
			bottomRight = new Vector2(colliderBounds.center.x - colliderBounds.extents.x + smallDistance, colliderBounds.center.y - colliderBounds.extents.y + smallDistance);
			if(Physics2D.OverlapArea(upLeft, bottomRight, GroundMask.value))
			{
				isSliding = 1;
				dropOffSlideTimer = 0;
			}
		}
		if (isInAir && !inputState.jumpDown && vel.y > breakJumpForce)
		{
			vel.y = breakJumpForce;
		}
		if(!isInAir)
		{
			walkGasCloudTimer += Mathf.Abs(vel.x * Time.deltaTime);
			if(walkGasCloudTimer > walkGasCloudRate)
			{
				walkGasCloudTimer -= walkGasCloudRate;
				Instantiate(walkGasCloud, buttPos.position, Quaternion.identity);
			}
		}
		rigidbody2D.velocity = vel;
	}

	private void UpdateAnimation()
	{
		Vector2 vel = rigidbody2D.velocity;
		if(!isInAir && Mathf.Abs(vel.x) > 0.1f)
		{
			transform.localScale = new Vector3(vel.x > 0 ? 1 : -1,1,1);
		}
		if(!isInAir)
		{
			if(Mathf.Abs(vel.x) > 0.1f)
			{
				//spriteAnimator.Play("Walk");
			}
			else
			{
				//spriteAnimator.Play("Idle");
			}
		}
		else if(isSliding != 0)
		{
			transform.localScale = new Vector3(isSliding,1,1);
			//spriteAnimator.Play("Slide");
		}
		else
		{
			
			if(vel.y > 128)
			{
				//spriteAnimator.Play("JumpUp");
			}
			else if(vel.y > -128)
			{
				//spriteAnimator.Play("JumpTop");
			}
			else
			{
				//spriteAnimator.Play("JumpDown");
			}
		}
	}
}
