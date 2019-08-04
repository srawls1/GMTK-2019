using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using FMODUnity;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator))]
public class CharacterController : MonoBehaviour
{
	[Header("Running")]
	[SerializeField] private float maxSpeed;
	[SerializeField] private float timeToFullSpeed;
	[SerializeField] private float timeToStop;

	[Header("Jumping")]
	[SerializeField] private float jumpHeight;
	[SerializeField] private float variableHangTime;
	[SerializeField] private float coyoteTime;
	[SerializeField] private float inputQueueTime;

	[Header("Dash")]
	[SerializeField] private float dashSpeed;
	[SerializeField] private float dashControlLossDuration;
	[SerializeField] private float dashDuration;
	[SerializeField, Tooltip("The length of time that the game pauses when you start a dash")] private float pauseDuration;
	[SerializeField] private float dashCooldownTime;

	[Header("Deflect")]
	[SerializeField] private float deflectRadius;
	[SerializeField] private float deflectSlowdownTime;
	[SerializeField] private float deflectRecoilSpeed;

	[Header("Bullet")]
	[SerializeField] private PlayerBullet bulletPreFab;
	[SerializeField] private TrackingLine laser;
	[SerializeField] private float timeBeforeBulletPickup;
	[SerializeField] private float spawnDistance = 0.5f;

	private new Rigidbody2D rigidbody;
	private Animator animator;
	private new Collider2D collider;
	private float acceleration;
	private float timeLeftGround;
	private float jumpQueuedUntil;
	private float lastDash;
	private bool facingRight = true;
	private bool hasBullet = true;
	private bool isChargingShot = false;
	private float timeShotBullet;

	public bool Charging
	{
		get; private set;
	}


	void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		collider = GetComponent<Collider2D>();
	}
	
	void Update()
	{
		UpdateIsOnGround();
		CheckForJump();
		CheckForDash();
		CheckForDeflect();
		CheckForShoot();
		ApplyHorizontalAcceleration();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerBullet bullet = collision.GetComponent<PlayerBullet>();
		if (Time.time > timeShotBullet + timeBeforeBulletPickup && bullet != null)
		{
			Destroy(bullet.gameObject);
			hasBullet = true;
			return;
		}
	}

	private void CheckForDeflect()
	{
		if (!Charging && Input.GetButton("Deflect"))
		{
			Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			float angle = Mathf.Atan2(direction.y, direction.x);
			angle /= Mathf.PI / 4;
			angle = Mathf.Round(angle);
			animator.SetInteger("DeflectDirection", Mathf.RoundToInt(angle));
			animator.SetTrigger("Deflect");

            angle *= Mathf.PI / 4;
			direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			StartCoroutine(DeflectRoutine(direction));
		}
	}


	private IEnumerator DeflectRoutine(Vector2 direction)
	{
		Vector2 position = transform.position;
		position += direction * deflectRadius * 0.5f;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(position, deflectRadius);
		for (int i = 0; i < colliders.Length; ++i)
		{
			HomingProjectile projectile = colliders[i].GetComponent<HomingProjectile>();
			if (projectile != null)
			{
				// TODO - Show particle or screen shader effect
				Time.timeScale = 1f;
				yield return new WaitForSecondsRealtime(deflectSlowdownTime);
				Time.timeScale = 1f;

				projectile.GetDeflected(direction);
				rigidbody.velocity = -direction.normalized * deflectSlowdownTime;
                //FMODUnity.RuntimeManager.PlayOneShot("event:/player/player_deflect");
            }
		}
	}

	private void CheckForDash()
	{
		if (lastDash + dashCooldownTime < Time.time && Input.GetButtonDown("Dash")) // We are not in a dash cooldown
		{
			Dash();
		}
		
	}

	private void CheckForShoot()
	{
		if (hasBullet && Input.GetButtonDown("Fire"))
		{
			StartCoroutine(Shoot());
		}

	}

	private void Dash()
	{
		acceleration = 0f;
		lastDash = Time.time;
		Vector2 direction;

		/*
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), IsOnGround() ? 0f : Input.GetAxisRaw("Vertical"));
		if (input.magnitude < 0.1f)
		{
			direction = facingRight ? Vector2.right : Vector2.left;
		}
		else
		{
			float angle = Mathf.Atan2(input.y, input.x);
			angle /= Mathf.PI / 4;
			angle = Mathf.Round(angle);
			angle *= Mathf.PI / 4;
			direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		}
		*/

		direction = GetCursorAngle();

		StartCoroutine(DashRoutine(direction));
	}

	private IEnumerator DashRoutine(Vector2 direction)
	{
		Time.timeScale = 0f;
		yield return new WaitForSecondsRealtime(pauseDuration);
		Time.timeScale = 1f;

		Vector2 velocity = direction * dashSpeed;
		rigidbody.velocity = velocity;
		Charging = true;
        animator.SetBool("Charging", true);
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Player/player_dash");

        for (float timePassed = 0f; timePassed < dashControlLossDuration; timePassed += Time.deltaTime)
		{
			rigidbody.velocity = velocity;
			yield return null;
		}

		float remainingTime = dashDuration - dashControlLossDuration;
		for (float timePassed = 0f; timePassed < remainingTime; timePassed += Time.deltaTime)
		{
			rigidbody.velocity = velocity;
			float input = Input.GetAxisRaw("Horizontal");
			if (!Mathf.Approximately(input, 0f) && direction.x * input <= 0) // There is horizontal input, and it's not in the direction of the dash, so we cancel out of it
			{
				break;
			}
		}

		Charging = false;
		animator.SetBool("Charging", false);
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Player/player_dash_recover");
    }

	private void ApplyHorizontalAcceleration()
	{
		if (!Charging && !isChargingShot)
		{
			float input = Mathf.Round(Input.GetAxisRaw("Horizontal"));
			Vector2 velocity = rigidbody.velocity;
			float time = input == 0f ? timeToStop : timeToFullSpeed;

			velocity.x = Mathf.SmoothDamp(velocity.x, input * maxSpeed, ref acceleration, time);
			if (!float.IsNaN(velocity.x))
			{
				rigidbody.velocity = velocity;
			}

			if (velocity.x > 0.05f)
			{
				facingRight = true;
			}
			if (velocity.x < -0.05f)
			{
				facingRight = false;
			}
			animator.SetFloat("Speed", velocity.x);
		}
	}

	private void CheckForJump()
	{
		if (IsOnGround())
		{
			if (Input.GetButtonDown("Jump") || jumpQueuedUntil > Time.time) // Either they just pressed the jump button, or they just landed and a jump was queued
			{
				Jump();
			}
		}
		else // Can't jump. Just queue the input in case we're about to land
		{
			if (Input.GetButtonDown("Jump"))
			{
				jumpQueuedUntil = Time.time + inputQueueTime;
			}
		}
	}

	private bool IsOnGround()
	{
		return timeLeftGround + coyoteTime > Time.time; // This means we are either on the ground, or still within the coyote time window
	}

	private void Jump()
	{
		jumpQueuedUntil = Time.time - Time.deltaTime; // Unqueue the jump input by setting it into the past
		Vector2 velocity = rigidbody.velocity;

		float jumpSpeed = Mathf.Sqrt(2f * jumpHeight * Physics2D.gravity.magnitude);
		velocity.y = jumpSpeed;
		rigidbody.velocity = velocity;
        animator.SetTrigger("Jump");
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Player/player_jump");
    }

	private void UpdateIsOnGround()
	{
		Vector2 bounds = collider.bounds.extents;
		// This raycast downward just beyond the extent of the character's collider will check if the character is standing on something
		if (Physics2D.Raycast(transform.position, Vector2.down, bounds.y + 0.05f).collider != null)
		{
			animator.SetBool("OnFloor", true);
			// Update the time they left the ground to now. If they are not on the ground, this will have been last updated when they left.
			timeLeftGround = Time.time;
		}
		else
		{
			animator.SetBool("OnFloor", false);
		}
	}

	private Vector2 GetCursorAngle() {
			Vector2 start = rigidbody.position;
			Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			return (end - start).normalized;
	}

	private IEnumerator Shoot()
	{
		// Set animation
		float chargeTime = 0f;
		// is this by value or by reference
		float current_width = laser.current_width;
		while (Input.GetButton("Fire"))
		{
			isChargingShot = true;
			chargeTime += Time.deltaTime;
			//todo
			if (chargeTime < bulletPreFab.maxChargeTime)
			{
				float new_width = laser.current_width + 0.003f;
				laser.updateLineWidth(new_width);
			}

			yield return null;
		}
		// set animation

		isChargingShot = false;

		hasBullet = false;
		laser.updateLineWidth(current_width);
		timeShotBullet = Time.time;
		Vector2 start = transform.position;
		Vector2 direction = laser.direction;
		start += direction.normalized * spawnDistance;
		
		float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		PlayerBullet b = Instantiate(bulletPreFab, start, Quaternion.Euler(0, 0, rotZ)) as PlayerBullet;
		b.objToReturnTo = gameObject;
		b.chargeTime = chargeTime;
	}
}
