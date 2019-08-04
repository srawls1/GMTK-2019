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

	private GameObject playerCharacter;
	private new Rigidbody2D rigidbody;
	private float currentMaxSpeed;
	private float letOffEndTime;
	private List<Vector2> path;

	public bool deflecting
	{
		get { return Time.time < letOffEndTime; }
	}

	private void Awake()
	{
		playerCharacter = FindObjectOfType<CharacterController>().gameObject;
		rigidbody = GetComponent<Rigidbody2D>();
		currentMaxSpeed = baseMaxSpeed;
	}

	void Update()
	{
		if (!deflecting)
		{
			if (hasDirectPath(playerCharacter.transform.position))
			{
				path = new List<Vector2>();
				path.Add(playerCharacter.transform.position);
			}
			if (path == null || path.Count == 0 || !hasDirectPath(path[0]) ||
				Vector2.Distance(playerCharacter.transform.position, path[path.Count - 1]) > 1f)
			{
				path = NavMesh.instance.GetClosestPath(transform.position, playerCharacter.transform.position, NavTerrainTypes.Floor);
				//PrintPath(path);
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

    //RuntimeManager.PlayOneShot("event:/boss/boss_bullet_deflected"); // TODO find how to trigger the bullets deflecting sound

    private bool hasDirectPath(Vector2 point)
	{
		Vector2 loc = transform.position;
		Vector2 dir = point - loc;
		return Physics2D.Raycast(loc, dir, dir.magnitude).collider == null;
	}

	private void PrintPath(List<Vector2> path)
	{
		if (path == null)
		{
			Debug.Log("null");
			return;
		}

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
		currentMaxSpeed += speedUpStep;
		rigidbody.velocity = direction * currentMaxSpeed;
		letOffEndTime = Time.time + deflectLetOffTime;
		// TODO - Address losing the gained speed maybe
	}
}
