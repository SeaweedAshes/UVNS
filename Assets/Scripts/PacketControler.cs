using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketControler : MonoBehaviour
{
    TimeHandler TIME_HANDLER;

    PacketObject[] arrayOfpacket = null;
    int indexOfcurrentPacket = 0;

    public void SetArray(PacketObject[] array)
    {
        arrayOfpacket = array;
    }

    public void SetTimeHandler (TimeHandler handler)
    {
        TIME_HANDLER = handler;
    }

    void Update()
    {
        float current_Time = TIME_HANDLER.GetCurrentTime();

        // set packet(after 0.05 seconds) to be shown in order to smooth animation
        while(arrayOfpacket[indexOfcurrentPacket].GetStartTime() < current_Time + 0.05f)
        {
            if(indexOfcurrentPacket == arrayOfpacket.Length - 1)
                break;
            
            if(arrayOfpacket[indexOfcurrentPacket].GetEndTime() > current_Time)
                arrayOfpacket[indexOfcurrentPacket].gameObject.SetActive(true);
            indexOfcurrentPacket++;
        }
    }

}
