using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpeedrunOnOff : MonoBehaviour
{
    public void SetSpeedrun(bool isOn)
	{
		SpeedrunTimer.isASpeedrun = isOn;
	}
}
