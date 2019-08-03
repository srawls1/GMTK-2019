using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingLine : MonoBehaviour
{
	public float spawnDistance = 0.5f;
	public LineRenderer lineRenderer;
    public Rigidbody2D character;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		Vector2 start = character.position;
		Vector2 dest = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 direction = dest - start;
		start += direction.normalized * spawnDistance;

		lineRenderer.SetPosition(0,  start);
		lineRenderer.SetPosition(1, dest);
        
    }
}
