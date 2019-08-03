using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBullet : MonoBehaviour
{
	[SerializeField] private float maxSpeed;
	[SerializeField] private float speedUpStep;

	private new Rigidbody2D rigidbody;
	private Vector2 velocity;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		velocity = transform.right * maxSpeed;
		rigidbody.velocity = velocity;
		
	}

	void Update()
	{
		/* TODO i want this to shoot out in the direction of the mouse cursor */
		/*
		if (rigidbody.velocity.magnitude > currentMaxSpeed)
		{
			rigidbody.velocity = rigidbody.velocity.normalized * baseMaxSpeed;
		}
		*/
	}

    void OnCollisionEnter2D (Collision2D col) {
		//Debug.Log(col.collider.name);
		HomingProjectile homingarrow = col.collider.GetComponent<HomingProjectile>();

		if (homingarrow != null)
		{
			homingarrow.GetDeflected(velocity.normalized);
		}
    }
}
