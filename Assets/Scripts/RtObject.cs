using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //FOR Image

public class RtObject : MonoBehaviour
{

    public int node_id;
    public Vector3 position = new Vector3(0,0);
    public Slider Queue;
    public int current_Queue_Size=0;
    public float current_Time=0f;


    class Event
    {
        public float t;
        public int v;
    }

    int maxQvalue = 1;

    List<Event> listOfQevent = new List<Event>();
    List<Event> listOfallQevent = new List<Event>();

    public void EnQueue(float time, int value)
    {
        Event evt = new Event();
        evt.t = time;
        evt.v = value;
        // if(node_id == 2)
            // Debug.Log("NodeId: "+node_id+", time:"+time+", EnQueue: "+value);
        listOfQevent.Add(evt); 
    }

    public void DeQueue(float time, int value)
    {
        Event evt = new Event();
        evt.t = time;
        evt.v = (-1)*value;
        // if(node_id == 2)
            // Debug.Log("NodeId: "+node_id+", time:"+time+", DeQueue: "+value);
        listOfQevent.Add(evt);
    }


    // Start is called before the first frame update
    void Start()
    {
        Queue.value = 0f;
        int current_Queue=0;
        while(listOfQevent.Count != 0)
        {

            Event evt = new Event();
            evt.t = listOfQevent[0].t;
            evt.v = current_Queue + listOfQevent[0].v;
            current_Queue = evt.v;
            if(node_id == 2)
                Debug.Log("time: "+evt.t+",value: "+evt.v+", current_size: "+current_Queue);
            if(evt.v > maxQvalue)
                maxQvalue = evt.v;
            listOfallQevent.Add(evt);
            listOfQevent.RemoveAt(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        current_Time += Time.deltaTime;
        if(listOfallQevent.Count >= 2)
        {
            if(listOfallQevent[0].t <= current_Time && listOfallQevent[1].t >= current_Time)
                {
                    Queue.value = (float)(listOfallQevent[0].v)/maxQvalue;
                    current_Queue_Size = listOfallQevent[0].v;
                }
                else if(listOfallQevent[1].t < current_Time)
                {
                    while(listOfallQevent[1].t < current_Time)
                    {
                        Queue.value = (float)(listOfallQevent[0].v)/maxQvalue;
                        current_Queue_Size = listOfallQevent[0].v;
                        listOfallQevent.RemoveAt(0);
                        if(listOfallQevent.Count == 1)
                            break;
                    }
                }
        }
    }
}
