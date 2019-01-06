using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityFunction {

	
	static public float Normailze(float value, float min, float max)
	{
		return (value - min) / (max - min);
	}


}
