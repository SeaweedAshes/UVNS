using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NodeObject : MonoBehaviour
{
    [Header ("Information")]
    public int nodeID;
    public Vector3 position = new Vector3(0,0);
    public float currentTime;

    [Header ("Absolute Value")]
    public int TxBuffer;
    public int RxBuffer;
    public int CongetstionWindow;

    [Space (20f)]
    
    [Header ("Slider")]
    public Slider TxBufferSlider;
    public Slider RxBufferSlider;
    public Slider CongetstionWindowSlider;

    [Header ("Image")]
    public SpriteRenderer spriteRenderer;
    public Sprite image_user;
    public Sprite image_satellite;
    public Sprite image_antenna;
    public Sprite image_server;
    public Sprite image_computer;


    public class Event
    {
        public float t;
        public int v;
    }


    TimeHandler TIME_HANDLER;

    int maxTxvalue = 1;
    int maxRxvalue = 1;
    int maxCwvalue = 1;

    List<Event> listOfTxevent = new List<Event>();
    List<Event> listOfRxevent = new List<Event>();
    List<Event> listOfCwevent = new List<Event>();

    Event[] arrayOfTxevent = null;
    Event[] arrayOfRxevent = null;
    Event[] arrayOfCwevent = null;

    int currentTxIndex = 0;
    int currentRxIndex = 0;
    int currentCwIndex = 0;

    public void AddEvent(List<Event> eventList, float time, int value, ref int maxValue)
    {
        Event evt = new Event();
        evt.t = time;
        evt.v = value;
        if (maxValue < value)
        {
            maxValue = value;
        }
        eventList.Add(evt);
    }

    public void AddTx(float time, int value)
    {
        AddEvent(listOfTxevent, time, value, ref maxTxvalue);

    }

    public void AddRx(float time, int value)
    {
        AddEvent(listOfRxevent, time, value, ref maxRxvalue);

    }

    public void AddCw(float time, int value)
    {
        AddEvent(listOfCwevent, time, value, ref maxCwvalue);

    }

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

    public void SetImage(int imageIndex)
    {
        switch(imageIndex)
        {
            case 1:
                spriteRenderer.sprite = image_user;
                break;
            case 2:
                spriteRenderer.sprite = image_satellite;
                break;
            case 3:
                spriteRenderer.sprite = image_antenna;
                break;
            case 4:
                spriteRenderer.sprite = image_server;
                break;
            case 5:
                spriteRenderer.sprite = image_computer;
                break;
            default:
                break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        TxBufferSlider.value = 0f;
        RxBufferSlider.value = 0f;
        CongetstionWindowSlider.value = 0f;

        arrayOfTxevent = listOfTxevent.ToArray();
        arrayOfRxevent = listOfRxevent.ToArray();
        arrayOfCwevent = listOfCwevent.ToArray();
    }
    

    void UpdateValue(float currentTime, Event[] arrayOfEvent, Slider slider, ref int currentIndex, int maxValue)
    {
        if (arrayOfEvent.Length != 0)
        {
            if (arrayOfEvent[currentIndex].t >= currentTime)
            {
                slider.value = (float)(arrayOfEvent[currentIndex].v) / maxValue;
            }
            else
            {
                while (currentIndex < arrayOfEvent.Length - 1 && arrayOfEvent[currentIndex].t < currentTime)
                {
                    currentIndex++;
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        currentTime = TIME_HANDLER.GetCurrentTime();
        
        UpdateValue(currentTime, arrayOfTxevent, TxBufferSlider, ref currentTxIndex, maxTxvalue);
        UpdateValue(currentTime, arrayOfRxevent, RxBufferSlider, ref currentRxIndex, maxRxvalue);
        UpdateValue(currentTime, arrayOfCwevent, CongetstionWindowSlider, ref currentCwIndex, maxCwvalue);

    }
}
