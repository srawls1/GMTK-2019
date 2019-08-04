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

	public bool returning
	{
		get { return Time.time > shotTime + timeBeforeStartReturning; }
	}

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
		if (returning)
		{
			Vector2 diff = objToReturnTo.transform.position - transform.position;
			rigidbody.AddForce(diff.normalized * returnForce);
			if (velocity.magnitude > maxSpeed)
			{
				velocity = velocity.normalized * maxSpeed;
				rigidbody.velocity = velocity;
			}
		}
	}

    void OnTriggerEnter2D (Collider2D col) {
		Debug.Log(col.name);
		//Debug.Log(col.collider.name);
		HomingProjectile homingarrow = col.GetComponent<HomingProjectile>();

		if (homingarrow != null)
		{
			homingarrow.GetDeflected(velocity.normalized);
		}
    }
}
