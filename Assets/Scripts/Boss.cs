using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Boss : MonoBehaviour
{
	[Header("Stats")]
	[SerializeField] private float maxHealth;
    [SerializeField] private float bossInvulTime;
	[SerializeField] private float timeToSummonArrow;

	[Header("Teleporting")]
	[SerializeField] private List<Transform> teleportPoints;
	[SerializeField] private float teleportTime;
	[SerializeField] private float teleportFrequency;

    [Header("Items")]
    [SerializeField] private HomingProjectile missile;

	new private Collider2D collider;
	private float timeShotMissile;
    public float health;
	private HomingProjectile currentProjectile;
	private int currentLocation;
	bool workingOnSummon;

    void Awake() {
		collider = GetComponent<Collider2D>();
		GetLocation();
        // Set health
        health = maxHealth;
        // Spawn homing missile
        spawnProjectile();
    }

    void Update(){
		if (currentProjectile == null)
		{
			StartCoroutine(spawnProjectile());
		}
    }
    
    IEnumerator spawnProjectile() {
		if (workingOnSummon)
		{
			yield break;
		}

		workingOnSummon = true;

		yield return StartCoroutine(Teleport());

		// TODO - animation?
		yield return new WaitForSeconds(timeToSummonArrow);

        // Make sure the boss is immune to its own projectile when spawning it
        timeShotMissile = Time.time;
        currentProjectile = Instantiate(missile, transform.position, Quaternion.identity);
		workingOnSummon = false;
    }

	private IEnumerator Teleport()
	{
		// TODO - wait for disappear animation
		collider.enabled = false;
		yield return new WaitForSeconds(teleportTime);
		int newLocation = health > 3 ?
			Random.Range(0, teleportPoints.Count - 2) :
			Random.Range(teleportPoints.Count - 2, teleportPoints.Count);
		transform.position = teleportPoints[newLocation].position;
		currentLocation = newLocation;
		collider.enabled = true;
		// TODO - wait for reapper animation
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (Time.time > timeShotMissile + bossInvulTime)
        {
			HomingProjectile ownProj = other.GetComponent<HomingProjectile>();
			if (ownProj != null)
			{
				Destroy(other.gameObject);
				takeDamage(GetDeflectDamage(ownProj));
			}

			PlayerBullet playerProj = other.GetComponent<PlayerBullet>();
			if (playerProj != null)
			{
				takeDamage(GetDeflectDamage(playerProj));
			}
            Debug.Log("Hit boss with projectile: " + other.name);
        }
    }

	private void GetLocation()
	{
		float minDist = Mathf.Infinity;
		for (int i = 0; i < teleportPoints.Count; ++i)
		{
			float dist = Vector2.Distance(transform.position, teleportPoints[i].position);
			if (dist < minDist)
			{
				minDist = dist;
				currentLocation = i;
			}
		}
	}

	private float GetDeflectDamage(HomingProjectile ownProj)
	{
		if (!ownProj.deflecting)
		{
			return 1f;
		}

		if (ownProj.GetComponent<Rigidbody2D>().velocity.magnitude > 15)
		{
			return 3f;
		}

		return 2f;
	}

	private float GetDeflectDamage(PlayerBullet playerProj)
	{
		if (playerProj.returning)
		{
			return 1f;
		}

		else if (playerProj.GetComponent<Rigidbody2D>().velocity.magnitude > 15)
		{
			return 2f;
		}

		return 1f;
	}

	void takeDamage(float damage) {
		Debug.Log("Boss took damage: " + damage);
        health -= damage;
        // TODO: probably play some damage animation here
        if (health <= 0) {
            die();
        }

		teleportPoints.RemoveAt(currentLocation);
		StartCoroutine(Teleport());
    }

    void die() {
        Debug.Log("Boss is dead");
        Destroy(gameObject);
		Destroy(currentProjectile.gameObject);
    }
}

