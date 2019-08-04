using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using FMODUnity;

public class PlayerLives : MonoBehaviour
{
	[SerializeField] private float baseMaxHealth;
    //[SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private Image healthBar;
    // Start is called before the first frame update

    private float current_health;

    void Start()
    {
        current_health = baseMaxHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = current_health / baseMaxHealth;
    }

    void OnTriggerEnter2D(Collider2D other) {
		Debug.Log(other.name);
        HomingProjectile homingprojectile = other.GetComponent<HomingProjectile>();
        if (homingprojectile != null && !homingprojectile.deflecting) {
            Debug.Log("Lost 1 health");
            current_health -= 1;
            //RuntimeManager.PlayOneShot("event:/player/player_damaged");
            Destroy(other.gameObject);
			if (current_health == 0)
			{
				Debug.Log("Player is dead");
			}
        }
    }
}
