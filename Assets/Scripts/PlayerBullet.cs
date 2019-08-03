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
		Vector2 force = new Vector2(15.0f, 0.0f);
		rigidbody.AddForce(force);
		/*
		if (rigidbody.velocity.magnitude > currentMaxSpeed)
		{
			rigidbody.velocity = rigidbody.velocity.normalized * baseMaxSpeed;
		}
		*/
	}

    void OnCollisionEnter2D (Collision2D col) {
        Debug.Log("Hello");
        Destroy(gameObject);
    }
}
