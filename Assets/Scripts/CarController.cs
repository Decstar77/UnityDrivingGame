using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarController : MonoBehaviour
{

	[SerializeField] private float SpeedFactor = 8f;
	[SerializeField] private float TorgueFactor = 200f;
	[SerializeField] private float DriftFactorStick = 0.9f;
	[SerializeField] private float DriftFactorSlip = 1f;
	[SerializeField] private float MaxStick = 2.5f;
	[SerializeField] private float UndersteerValue = 5;



	private Vector3 JoyStickDirection;


	float vertInp = 0;
	float hortInp = 0;
	private Rigidbody2D ri;
	


	void Start()
	{
		ri = GetComponent<Rigidbody2D>();
		ri.gravityScale = 0;	

	}

	private void FixedUpdate()
	{
		
		Vector2 direction = new Vector2(JoyStickDirection.x, JoyStickDirection.y);
		float acceleration = UtilityFunction.Normailze(JoyStickDirection.magnitude, 0, 100);
		direction.Normalize();
		




		float toAngle = ((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg)) - 90;
		float fromAngle = transform.rotation.eulerAngles.z;
		float netAngle = toAngle -fromAngle;
		if (netAngle == -360 || netAngle == 360)
			netAngle = 0;






		ClampVelocity(5);
		ri.AddRelativeForce(Vector2.up * SpeedFactor * acceleration);
		if (direction != Vector2.zero)
		{
			print("adding rotation");
			transform.Rotate(new Vector3(0, 0, netAngle));
		}
		else
		{
			ri.angularVelocity = 0;
		}
		//////////////////Do Steering //////////////
		////////////////////Do drift cals///////////////
		float driftFactor = DriftFactorStick;
		if (SideWaysVelocity().magnitude > MaxStick)
			driftFactor = DriftFactorSlip;
		ri.velocity = ForwardVelocity() + SideWaysVelocity() * driftFactor;




	}

	private Vector2 ForwardVelocity()
	{
		return transform.up * Vector2.Dot(ri.velocity, transform.up);
	}
	private Vector2 SideWaysVelocity()
	{
		return transform.right * Vector2.Dot(ri.velocity, transform.right);
	}
	private void ClampVelocity(float MaxSpeed)
	{
		if (ri.velocity.magnitude > MaxSpeed)
		{
			ri.velocity = Vector2.ClampMagnitude(ri.velocity, MaxSpeed);
		}
	}
	public void SetJoyStickInput(Vector3 vec)
	{
		JoyStickDirection = vec;
	}
	
}