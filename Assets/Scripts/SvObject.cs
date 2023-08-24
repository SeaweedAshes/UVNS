using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //FOR Image


public class SvObject : MonoBehaviour
{
 
    public int node_id;
    public Vector3 position = new Vector3(0,0);
    public Slider Txbuffer;
    public Slider Rxbuffer;
    public Slider Cw;


    public float current_Time=0f;

    public float current_Tx_Size;
    public float current_Rx_Size;
    public float current_CW_Size;

    class Event
    {
        public float t;
        public int v;
    }

    int maxTxvalue = 1;
    int maxRxvalue = 1;
    int maxCWvalue = 1;

    List<Event> listOfTxevent = new List<Event>();
    List<Event> listOfRxevent = new List<Event>();
    List<Event> listOfCwevent = new List<Event>();

    public void AddTx(float time, int value)
    {
        Event evt = new Event();
        evt.t = time;
        evt.v = value;
        if(maxTxvalue<value)
        {
            maxTxvalue = value;
        }
        listOfTxevent.Add(evt); 
    }

    public void AddRx(float time, int value)
    {
        Event evt = new Event();
        evt.t = time;
        evt.v = value;
        if(maxRxvalue<value)
        {
            maxRxvalue = value;
        }
        listOfRxevent.Add(evt); 
    }

    public void AddCw(float time, int value)
    {
        Event evt = new Event();
        evt.t = time;
        evt.v = value;
        if(maxCWvalue<value)
        {
            maxCWvalue = value;
        }
        listOfCwevent.Add(evt); 
    }

    // Start is called before the first frame update
    void Start()
    {
        Txbuffer.value = 0f;
        Rxbuffer.value = 0f;
        Cw.value = 0f;
        //buffer.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        //buffer.transform.position = position+new Vector3(0f,-0.3f,0f);

    }

    // Update is called once per frame
    void Update()
    {
        current_Time += Time.deltaTime;
        if(listOfTxevent.Count >= 2)
        {
            if(listOfTxevent[0].t <= current_Time && listOfTxevent[1].t >= current_Time)
                {
                    Txbuffer.value = (float)(listOfTxevent[0].v)/maxTxvalue;
                    current_Tx_Size = listOfTxevent.Count;
                }
                else if(listOfTxevent[1].t < current_Time)
                {
                    while(listOfTxevent[1].t < current_Time)
                    {
                        listOfTxevent.RemoveAt(0);
                        if(listOfTxevent.Count == 1)
                            break;
                    }
                }
        }

        if(listOfRxevent.Count >= 2)
        {
            if(listOfRxevent[0].t <= current_Time && listOfRxevent[1].t >= current_Time)
                {
                    Rxbuffer.value = (float)(listOfRxevent[0].v)/maxRxvalue;
                    current_Rx_Size = listOfRxevent.Count;
                }
                else if(listOfRxevent[1].t < current_Time)
                {
                    while(listOfRxevent[1].t < current_Time)
                    {
                        listOfRxevent.RemoveAt(0);
                        if(listOfRxevent.Count == 1)
                            break;
                    }
                }
        }
        if(listOfCwevent.Count >= 2)
        {
            if(listOfCwevent[0].t <= current_Time && listOfCwevent[1].t >= current_Time)
                {
                    Cw.value = (float)(listOfCwevent[0].v)/maxCWvalue;
                    current_CW_Size = listOfCwevent.Count;
                }
                else if(listOfCwevent[1].t < current_Time)
                {
                    while(listOfCwevent[1].t<current_Time)
                    {
                        listOfCwevent.RemoveAt(0);
                        if(listOfCwevent.Count == 1)
                            break;
                    }
                }
        }


        //StartCoroutine(BufferUpdate());
    }
}
