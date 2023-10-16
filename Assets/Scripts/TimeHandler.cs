using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 


public class TimeHandler : MonoBehaviour
{
    // You must set the following in the inspector window
    public Slider timeSpeedSlider;
    public Slider currentTimeSlider;
    public Text currentTimeText;  

    float simuCurrentTime = 0f;
    float simEndTime = 0f;

    public void SetEndTime(float t)
    {
        simEndTime = t;
    }

    public float GetCurrentTime()
    {
        return simuCurrentTime;
    }

    public float GetEndTime()
    {
        return simEndTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        if(simuCurrentTime<simEndTime)
        {
            currentTimeText.text = simuCurrentTime.ToString();
            // Set timeScale to timeSpeedSlider.value (range of timeSpeedSlider.value is 0~1,000,000) 
            Time.timeScale = ((timeSpeedSlider.value)/500000);

            currentTimeSlider.value = simuCurrentTime/simEndTime;
        }  
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        simuCurrentTime += Time.deltaTime;
    }

    void init()
    {
        // Set value timeSpeedSlider to 0.0f
        timeSpeedSlider.value = 0.0f;
    }
}
