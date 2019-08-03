using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBullet : MonoBehaviour
{
	[SerializeField] private float maxSpeed;
	[SerializeField] private float timeBeforeStartReturning;
	[SerializeField] private float returnForce;

	public GameObject objToReturnTo;

	private new Rigidbody2D rigidbody;
	private Vector2 velocity;
	private float shotTime;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		velocity = transform.right * maxSpeed;
		rigidbody.velocity = velocity;
		shotTime = Time.time;
	}

	void Update()
	{
		velocity = rigidbody.velocity;
		if (Time.time > shotTime + timeBeforeStartReturning)
		{
			Vector2 diff = objToReturnTo.transform.position - transform.position;
			rigidbody.AddForce(diff.normalized * returnForce);
		}
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
