using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingProjectile : MonoBehaviour
{
	[SerializeField] private float maxSpeed;
	[SerializeField] private float forceTowardPlayer;

	[SerializeField] private GameObject playerCharacter;

	private new Rigidbody2D rigidbody;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		Vector2 distFromPlayer = playerCharacter.transform.position - transform.position;
		Vector2 force = distFromPlayer.normalized * forceTowardPlayer - rigidbody.velocity;
		rigidbody.AddForce(force);
		if (rigidbody.velocity.magnitude > maxSpeed)
		{
			rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
		}
	}
}
