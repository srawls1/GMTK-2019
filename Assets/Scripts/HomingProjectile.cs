using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingProjectile : MonoBehaviour
{
	[SerializeField] private float baseMaxSpeed;
	[SerializeField] private float forceTowardPlayer;
	[SerializeField] private float speedUpStep;
	[SerializeField] private float deflectLetOffTime;
	[SerializeField] private GameObject playerCharacter;

	private new Rigidbody2D rigidbody;
	private float currentMaxSpeed;
	private float letOffEndTime;
	private List<Vector2> path;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		currentMaxSpeed = baseMaxSpeed;
	}

	void Update()
	{
		if (Time.time > letOffEndTime)
		{
			if (path == null || path.Count == 0 || !hasDirectPath(path[0]) ||
				Vector2.Distance(playerCharacter.transform.position, path[path.Count - 1]) > 1f)
			{
				path = NavMesh.instance.GetClosestPath(transform.position, playerCharacter.transform.position, NavTerrainTypes.Floor);
				PrintPath(path);
			}
			if (path == null || path.Count == 0)
			{
				path = new List<Vector2>();
				path.Add(playerCharacter.transform.position);
			}

			Vector2 towardNext = path[0] - (Vector2)transform.position;
			Vector2 force = towardNext.normalized * forceTowardPlayer - rigidbody.velocity;
			rigidbody.AddForce(force);
			if (rigidbody.velocity.magnitude > currentMaxSpeed)
			{
				rigidbody.velocity = rigidbody.velocity.normalized * baseMaxSpeed;
			}

			Vector2 second = path.Count > 1 ? path[1] : (Vector2)playerCharacter.transform.position;
			if (hasDirectPath(second))
			{
				path.RemoveAt(0);
			}
		}
	}

	private bool hasDirectPath(Vector2 point)
	{
		Vector2 loc = transform.position;
		Vector2 dir = point - loc;
		return Physics2D.Raycast(loc, dir, dir.magnitude).collider == null;
	}

	private void PrintPath(List<Vector2> path)
	{
		StringBuilder builder = new StringBuilder("[");
		bool commaNeeded = false;

		foreach (Vector2 v in path)
		{
			if (commaNeeded)
			{
				builder.Append(", ");
			}
			builder.Append(v);
			commaNeeded = true;
		}

		builder.Append("]");
		Debug.Log(builder.ToString());
	}

	public void GetDeflected(Vector2 direction)
	{
		Debug.Log(direction);
		currentMaxSpeed += speedUpStep;
		rigidbody.velocity = direction * currentMaxSpeed;
		letOffEndTime = Time.time + deflectLetOffTime;
		// TODO - Address losing the gained speed maybe
	}
}
