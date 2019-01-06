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
	private bool accelerteButtonStatus = false;
	void Start()
	{
		TurnDirection = VelocityDirection = 0;
		TurnAmount = VelocityAmount = 0;
		ri = GetComponent<Rigidbody2D>();
		ri.gravityScale = 0;
		

	}

	private void FixedUpdate()
	{
		float maxVelo = 40;
		Vector2 dir = new Vector2(JoyDirection.x, JoyDirection.y);
		Vector2 pos = new Vector2(transform.position.x, transform.position.y);

		float toAngle = ((Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg)) - 90;
		float fromAngle = transform.rotation.eulerAngles.z;

		float netAngle = toAngle -fromAngle;
		if (netAngle == -360 || netAngle == 360)
			netAngle = 0;
		if (ri.velocity.magnitude > 5)
		{
			ri.velocity = Vector2.ClampMagnitude(ri.velocity, 5);
		}
		//print("To" + toAngle);

		//print(netAngle);
		//netAngle *= Normailze(ri.velocity.magnitude, 0, 25);
		//print(toAngle);
		//fromAngle = Mathf.Lerp(fromAngle, toAngle, 0.99f);
		if (!accelerteButtonStatus)
			vertInp = Input.GetAxis("Vertical");
		else
			vertInp = 1;
		ri.AddRelativeForce(Vector2.up * SpeedFactor * vertInp);
		//////////////////Do Steering //////////////
		vertInp = Mathf.Clamp(vertInp, -0.3f, 1);
		float tf = Mathf.Lerp(0, TorgueFactor, ri.velocity.magnitude / UndersteerValue);
		//Debug.Log(vertInp);

		float lep = UtilityFunction.Normailze(ri.velocity.magnitude, 0, 5);
		print(lep * netAngle);
		if (dir != Vector2.zero)
			transform.Rotate(new Vector3(0, 0, netAngle * lep));
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
	public void AccelerateFoward(bool isDown)
	{
		this.accelerteButtonStatus = isDown;
	}
}