using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketObject1 : MonoBehaviour
{
    public Vector3 start_Position;
    public Vector3 end_Position;
    public float start_Time;
    public float end_Time;
    public float duration;
    
    public GameObject forwardPacketPrefab;
    public GameObject backwardPacketPrefab;

    public int start_nodeID;
    public int end_nodeID;

    public float current_Time=0f;

    private int direction;
    public void setDirection(int d)
    {
        direction=d;
    }

    GameObject packet;

    void Start()
    {
            if(packet==null&&direction == 0)
                packet = Instantiate(forwardPacketPrefab,GameObject.Find("Canvas").transform);
            else if(packet==null&&direction == 1)
                packet = Instantiate(backwardPacketPrefab,GameObject.Find("Canvas").transform);
            packet.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(current_Time >= start_Time && current_Time <= end_Time)
        {
            packet.SetActive(true);
            AnimateObject();
        }
        else if(end_Time < current_Time )
        {
            if(packet!=null)
            {   
                Destroy(packet);
            }
            Destroy(this.gameObject);
            //Destroy(this.gameObject);
            //gameObject.SetActive(false);
        }
        else
        {
            current_Time += Time.deltaTime;
        }
    }

    void AnimateObject( )
    {
   
         // Calculate the normalized time (0 to 1) based on the current time and duration
         float normalizedTime = (current_Time - start_Time) / duration;

            // Update the object's position based on the starting and ending positions
            packet.transform.position = Vector3.Lerp(start_Position, end_Position, normalizedTime);

            current_Time += Time.deltaTime;
    }
}
