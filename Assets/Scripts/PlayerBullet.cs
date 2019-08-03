using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBullet : MonoBehaviour
{
	[SerializeField] private float baseMaxSpeed;
	[SerializeField] private float speedUpStep;

	private new Rigidbody2D rigidbody;
	private float currentMaxSpeed;

	private void Start()
	{
		baseMaxSpeed = 5;
		speedUpStep = 2;	
	}


	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		currentMaxSpeed = baseMaxSpeed;
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
		Debug.Log(col.collider.name);
		HomingProjectile homingarrow = col.collider.GetComponent<HomingProjectile>();

		if (homingarrow != null)
		{
			Debug.Log("Hello");
			// find the vector of this object
			Vector2 vel = rigidbody.velocity;
			Debug.Log(vel);
			homingarrow.GetDeflected(vel);
		}
    }
}
