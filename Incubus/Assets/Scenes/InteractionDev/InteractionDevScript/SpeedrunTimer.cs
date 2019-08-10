using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedrunTimer : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI textmeshPro;
	public static bool isASpeedrun = false;
	private bool started = false;
	private bool finished = false;
	private DateTime speedrunStart = DateTime.Now;
	private DateTime finishTime = DateTime.Now;
	

    // Start is called before the first frame update
    void Start()
    {
        if (isASpeedrun)
		{
			textmeshPro.enabled = true;
		}
		else
		{
			textmeshPro.enabled = false;
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (!isASpeedrun)
		{
			return;
		}

		if (!finished
			&& (Input.GetButtonDown("Horizontal")
			|| (Input.GetButtonDown("Vertical"))
			|| (Math.Abs(Input.GetAxis("Horizontal")) > 0.1f)
			|| (Math.Abs(Input.GetAxis("Vertical")) > 0.1f)))
		{
			StartSpeedrun();
		}

		UpdateUI();

        UpdateAchievement();
    }

	private void StartSpeedrun()
	{
		if (!started)
		{
			started = true;
			speedrunStart = DateTime.Now;
		}
	}

	private void UpdateUI()
	{
		long elapsedTicks = DateTime.Now.Ticks - speedrunStart.Ticks;
		TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);

		if (!started)
		{
			elapsedSpan = new TimeSpan(0);
		}
		else if (finished)
		{
			elapsedSpan = new TimeSpan(finishTime.Ticks - speedrunStart.Ticks);
		}

		string timeString = "";

		if (elapsedSpan.Hours < 1)
		{
			timeString = ""
			+ elapsedSpan.Minutes.ToString("D2") + ":"
			+ elapsedSpan.Seconds.ToString("D2") + ":"
			+ elapsedSpan.Milliseconds.ToString("D3");
		}
		else
		{
			timeString = "XX:XX:XX";
		}
		
		//Debug.Log(timeString);
		textmeshPro.SetText(timeString);
	}

	private void Finish()
	{
		if (!finished)
		{
			finished = true;
			finishTime = DateTime.Now;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isASpeedrun)
		{
			return;
		}

		if (other.tag == "speedrunFinish")
		{
			Finish();
		}
	}

    private void UpdateAchievement()
    {
        if (finished)
        {
            TimeSpan elapsedSpan = new TimeSpan(finishTime.Ticks - speedrunStart.Ticks);
            if (elapsedSpan.Minutes <= 12)
            {
                Achievement_Manager.Set_TWELVE_MINUTES();
            }
            if (elapsedSpan.Minutes <= 10)
            {
                Achievement_Manager.Set_TEN_MINUTES();
            }
            if (elapsedSpan.Minutes <= 8)
            {
                Achievement_Manager.Set_EIGHT_MINUTES();
            }
        }
    }
}
