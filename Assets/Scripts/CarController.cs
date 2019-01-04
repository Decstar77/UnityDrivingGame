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
	[SerializeField] private bool SteeringRealism = false;



	private Vector3 JoyDirection;
	private float TurningRight;
	private float TurningLeft;
	private float TurnDirection;
	private float TurnAmount;
	
	private float Acceleration;
	private float Brake;
	private float VelocityDirection;
	private float VelocityAmount;

	float vertInp = 0;
	float hortInp = 0;
	private Rigidbody2D ri;

	void Start()
	{
		TurnDirection = VelocityDirection = 0;
		TurnAmount = VelocityAmount = 0;
		ri = GetComponent<Rigidbody2D>();
		ri.gravityScale = 0;
		

	}

	private void FixedUpdate()
	{
		print(JoyDirection);
		vertInp = Input.GetAxis("Vertical");
		hortInp = Input.GetAxis("Horizontal");

		vertInp = (JoyDirection.y);
		hortInp = (JoyDirection.x);

		ri.AddRelativeForce(Vector2.up * SpeedFactor * vertInp);
		//////////////////Do Steering //////////////
		vertInp = Mathf.Clamp(vertInp, -0.3f, 1);
		float tf = Mathf.Lerp(0, TorgueFactor, ri.velocity.magnitude / UndersteerValue);
		if (vertInp < 0 && SteeringRealism == true)
			ri.angularVelocity = (tf * hortInp);
		else
			ri.angularVelocity = (tf * hortInp * -1);
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
	private float DoSteeringInput()
	{
		TurnDirection = TurningRight - TurningLeft;
		TurnAmount = Mathf.Lerp(TurnAmount, TurnDirection, 0.2f);
		if (TurnAmount == -1 || TurnAmount == 1)
			return TurnAmount;
		return TurnAmount;
	}
	private float DoAccelerationInput()
	{
		VelocityDirection = Acceleration - Brake;
		VelocityAmount = Mathf.Lerp(VelocityAmount, VelocityDirection, 0.2f);
		if (VelocityAmount == -1 || VelocityAmount == 1)
			return VelocityAmount;
		return VelocityAmount;
	}


	public void SetJoyStickDirection(Vector3 vec)
	{
		JoyDirection = vec;
	}

}