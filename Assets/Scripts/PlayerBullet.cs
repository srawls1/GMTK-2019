using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBullet : MonoBehaviour
{
	[SerializeField] private float maxSpeed;
	[SerializeField] private float maxEffectiveChargeTime;
	[SerializeField] private float chargeTimeScale;
	[SerializeField] private float timeBeforeStartReturning;
	[SerializeField] private float returnForce;

	public GameObject objToReturnTo;
	public float chargeTime;

	private new Rigidbody2D rigidbody;
	private Vector2 velocity;
	private float shotTime;

	public float maxChargeTime {
		get { return maxEffectiveChargeTime; }
	}

	public bool returning
	{
		get { return Time.time > shotTime + timeBeforeStartReturning; }
	}

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		shotTime = Time.time;
	}
	private IEnumerator Start()
	{
		yield return null;
		Debug.Log(chargeTime);
		chargeTime = Mathf.Min(chargeTime, maxEffectiveChargeTime);
		Debug.Log(chargeTime);
		maxSpeed += chargeTimeScale * chargeTime;
		Debug.Log(maxSpeed);
		velocity = transform.right * maxSpeed;
		Debug.Log(velocity);
		rigidbody.velocity = velocity;
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
