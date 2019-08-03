using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Boss : MonoBehaviour
{
	[Header("Stats")]
	[SerializeField] private float maxHealth;
    [SerializeField] private float bossInvulTime;

    [Header("Items")]
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private HomingProjectile missile;
    
	private float timeShotMissile;
    public float health;

    void Awake() {
        // Set health
        health = maxHealth;
        // Spawn homing missile
        spawnProjectile();
    }


    void Update(){

    }
    
    void spawnProjectile() {
        // Make sure the boss is immune to its own projectile when spawning it
        timeShotMissile = Time.time;
        Instantiate(missile, rigidbody.position, rigidbody.rotation, rigidbody);
    }

    void onTriggerEnter2D(Collider2D other) {
		if (Time.time > timeShotMissile + bossInvulTime)
        {
            Debug.Log("Hit boss with projectile" + other.name);
            Destroy(other);
            // TODO: give every bullet a damage value. Get srawls to look at this.
            // How to access properties of that object?
            // This won't compile
            takeDamage(other.damage);
        }
    }

    void takeDamage(float damage) {
        current_health -= damage;
        // TODO: probably play some damage animation here
        if (current_health <= 0) {
            die();
        }
    }

    void die() {
        Debug.Log("Boss is dead");
        Destroy(gameObject);
    }
}

