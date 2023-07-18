using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class TimedBuilding : MonoBehaviour
{
    public bool inProgress;
    private DateTime timerStart;
    private DateTime timerEnd;

    [Header("Production Time")]
    public int days;
    public int hours;
    public int minutes;
    public int seconds;

    [Header("UI")]
    [SerializeField] private GameObject window;
    [SerializeField] private GameObject timeLeftObj;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private Slider timeLeftSlider;
    //[SerializeField] private Button skipButton;
    //[SerializeField] private Button startButton;

    Building buildObj;

    void InitializeWindow()
    {
        window.SetActive(true);

        timeLeftObj.SetActive(true);
        StartCoroutine(DisplayTime());
    }

   private IEnumerator DisplayTime()
    {
        DateTime start = DateTime.Now;
        TimeSpan timeLeft = timerEnd - start;
        double totalSecondsLeft = timeLeft.TotalSeconds;
        double totalSeconds = (timerEnd - timerStart).TotalSeconds;
        string text;

        while (window.activeSelf && timeLeftObj.activeSelf)
        {
            text = "";
            timeLeftSlider.value = 1 - Convert.ToSingle((timerEnd - DateTime.Now).TotalSeconds / totalSeconds);

            if(totalSecondsLeft > 1)
            {
                if(timeLeft.Days != 0)
                {
                    TimeSpan ts1 = TimeSpan.FromSeconds(totalSecondsLeft);
                    text += ts1.Days + "d ";
                    text += ts1.Hours + "h";
                    //yield return new WaitForSeconds(timeLeft.Minutes * 60);
                }
                else if(timeLeft.Hours != 0)
                {
                    TimeSpan ts2 = TimeSpan.FromSeconds(totalSecondsLeft);
                    text += ts2.Hours + "h ";
                    text += ts2.Minutes + "m";
                    //yield return new WaitForSeconds(timeLeft.Seconds);
                }
                else if(timeLeft.Minutes != 0)
                {
                    TimeSpan ts = TimeSpan.FromSeconds(totalSecondsLeft);
                    text += ts.Minutes + "m ";
                    text += ts.Seconds + "s";
                }
                else
                {
                    text += Mathf.FloorToInt((float) totalSecondsLeft) + "s"; 
                }

                timeLeftText.text = text;

                totalSecondsLeft -= Time.deltaTime;
                yield return null;
            }
            else
            {
                timeLeftText.text = "Finished";
                inProgress = false;
                timeLeftSlider.value = 1;
                buildObj = this.gameObject.GetComponent<Building>();
                buildObj.finishedBuilding = true;
                break;                
            }
        }

        yield return null;
    }

    //Timed Event
    public void StartTimer()
    {
        timerStart = DateTime.Now;
        TimeSpan time = new TimeSpan(days, hours, minutes, seconds);
        timerEnd = timerStart.Add(time);

        inProgress = true;
        StartCoroutine(Timer());
        InitializeWindow();
    }

    private IEnumerator Timer()
    {
        DateTime start = DateTime.Now;
        double secondsToFinish = (timerEnd - start).TotalSeconds;
        yield return new WaitForSeconds(Convert.ToSingle(secondsToFinish));

        inProgress = false;
        Debug.Log("terminado");
    } 
}
