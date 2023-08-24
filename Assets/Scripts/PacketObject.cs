using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketObject : MonoBehaviour
{
    public Vector3 start_Position;
    public Vector3 end_Position;
    public float start_Time;
    public float end_Time;
    public float duration;
    

    public int start_nodeID;
    public int end_nodeID;

    public float current_Time=0f;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if(current_Time >= start_Time && current_Time <= end_Time)
        {
            // if(gameObject.activeSelf == false)
            //     GameObject.Find("Canvas").transform.Find(gameObject.name).gameObject.SetActive(true);
            AnimateObject();
        }
        else if(end_Time < current_Time)
        {
            // Destroy(this.gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            current_Time += Time.deltaTime;
        }
    }

    void AnimateObject()
    {
   
         // Calculate the normalized time (0 to 1) based on the current time and duration
         float normalizedTime = (current_Time - start_Time) / duration;

            // Update the object's position based on the starting and ending positions
            transform.position = Vector3.Lerp(start_Position, end_Position, normalizedTime);

            current_Time += Time.deltaTime;
    }
}
