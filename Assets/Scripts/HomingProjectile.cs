using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingProjectile : MonoBehaviour
{
	[SerializeField] private float baseMaxSpeed;
	[SerializeField] private float forceTowardPlayer;
	[SerializeField] private float speedUpStep;
	[SerializeField] private GameObject playerCharacter;

	private new Rigidbody2D rigidbody;
	private float currentMaxSpeed;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		currentMaxSpeed = baseMaxSpeed;
	}

	void Update()
	{
		Vector2 distFromPlayer = playerCharacter.transform.position - transform.position;
		Vector2 force = distFromPlayer.normalized * forceTowardPlayer - rigidbody.velocity;
		rigidbody.AddForce(force);
		if (rigidbody.velocity.magnitude > currentMaxSpeed)
		{
			rigidbody.velocity = rigidbody.velocity.normalized * baseMaxSpeed;
		}
	}

	public void GetDeflected(Vector2 direction)
	{
		currentMaxSpeed += speedUpStep;
		rigidbody.velocity = direction * currentMaxSpeed;
		// TODO - Address losing the gained speed maybe
	}
}
