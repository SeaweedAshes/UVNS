using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RtObject : MonoBehaviour
{
    [Header ("Information")]
    public int nodeID;
    public Vector3 position = new Vector3(0,0);
    public float currentTime;

    [Space (20f)]
    
    [Header ("Slider")]
    public Slider QueueSlider;
    public int Queue;

    TimeHandler TIME_HANDLER;

    class Event
    {
        public float t;
        public int v;
    }

    int maxQvalue = 1;

    int currentIndex = 0;

    List<Event> listOfQevent = new List<Event>();
    List<Event> listOfallQevent = new List<Event>();

    Event[] arrayOfQevent = null;

    
    public void SetNodeID(int nodeid)
    {
        nodeID = nodeid;
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
    }

    public void SetTimeHandler(TimeHandler handler)
    {
        TIME_HANDLER = handler;
    }

    public void AddEnqueue(float time, int value)
    {
        Event evt = new Event();
        evt.t = time;
        evt.v = value;
        listOfQevent.Add(evt); 
    }

    public void AddDequeue(float time, int value)
    {
        Event evt = new Event();
        evt.t = time;
        evt.v = (-1)*value;
        listOfQevent.Add(evt);
    }


    // Start is called before the first frame update
    void Start()
    {
        QueueSlider.value = 0f;
        int currentQueue = 0;
        while(listOfQevent.Count != 0)
        {

            Event evt = new Event();
            evt.t = listOfQevent[0].t;
            evt.v = currentQueue + listOfQevent[0].v;
            currentQueue = evt.v;
            if(evt.v > maxQvalue)
                maxQvalue = evt.v;
            listOfallQevent.Add(evt);
            listOfQevent.RemoveAt(0);
        }
        arrayOfQevent = listOfallQevent.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = TIME_HANDLER.GetCurrentTime();
        if (arrayOfQevent.Length != 0)
        {
            if (arrayOfQevent[currentIndex].t >= currentTime)
            {
                QueueSlider.value = (float)(arrayOfQevent[currentIndex].v) / maxQvalue;
            }
            else
            {
                while (currentIndex < arrayOfQevent.Length - 1 && arrayOfQevent[currentIndex].t < currentTime)
                {
                    currentIndex++;
                }
            }
        }
    }
}
