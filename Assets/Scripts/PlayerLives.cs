using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLives : MonoBehaviour
{
	[SerializeField] private int baseMaxHealth;
    [SerializeField] private Rigidbody2D rigidbody;
    // Start is called before the first frame update

    private int current_health;

    void Start()
    {
        int current_health = baseMaxHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        HomingProjectile homingprojectile = other.collider.GetComponent<HomingProjectile>();
        if (homingprojectile != null) {
            Debug.Log("Lost 1 health");
            current_health -= 1;
            Destroy(other.gameObject);
        }
        // todo probably need to make the 
        // decrease the health bar
    }
}
