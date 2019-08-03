using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLives : MonoBehaviour
{
	[SerializeField] private int baseMaxHealth;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private Image healthBar;
    // Start is called before the first frame update

    private float current_health;

    void Start()
    {
        float current_health = baseMaxHealth;
        Debug.Log(current_health);
        Debug.Log(baseMaxHealth);
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(current_health); // how come this returns 0 here but it's 5 in Start? can't be scoping, right
        Debug.Log((current_health/baseMaxHealth));
        healthBar.fillAmount = current_health / baseMaxHealth;
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        HomingProjectile homingprojectile = other.collider.GetComponent<HomingProjectile>();
        if (homingprojectile != null) {
            Debug.Log("Lost 1 health");
            current_health -= 1;
            Destroy(other.gameObject);
        }
        // TODO build healthbar UI element
    }
}
