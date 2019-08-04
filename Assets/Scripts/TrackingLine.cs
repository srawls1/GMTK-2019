using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingLine : MonoBehaviour
{
	public float spawnDistance = 0.5f;
	public float laserLength = 5f;
	public LineRenderer lineRenderer;
    public Rigidbody2D character;

	public bool usingJoystick
	{
		get; private set;
	}

	public Vector2 direction
	{
		get; private set;
	}


	public float current_width;
	public void updateLineWidth(float linewidth) {
		lineRenderer.startWidth = linewidth;
		lineRenderer.endWidth = linewidth;

	}

	void Awake() {
		current_width = lineRenderer.startWidth;
	}
	

    // Update is called once per frame
    void Update()
    {
		current_width = lineRenderer.startWidth;
		Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		if (!Mathf.Approximately(mouseMovement.magnitude, 0))
		{
			usingJoystick = false;
		}

		bool currentInput = false;
		Vector2 aimInput = new Vector2(Input.GetAxisRaw("AimX"), Input.GetAxisRaw("AimY"));
		if (aimInput.magnitude >= 0.2f)
		{
			usingJoystick = true;
			currentInput = true;
		}

		Vector2 start = character.position;
		Vector2 end;
		if (usingJoystick)
		{
			if (currentInput)
			{
				direction = aimInput.normalized;
			}
			end = start + direction * laserLength;
		}
		else
		{
			end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			direction = (end - start).normalized;
		}
		
		start += direction.normalized * spawnDistance;

		lineRenderer.SetPosition(0,  start);
		lineRenderer.SetPosition(1, end);
        
    }
}
