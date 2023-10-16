using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PacketObject : MonoBehaviour
{
    [Header ("Information")]
    public int startNodeID;
    public int endNodeID;

    Vector3 startPosition;
    Vector3 endPosition;
    public float startTime;
    public float endTime;
    float duration;
    float currentTime=0f;

    TimeHandler TIME_HANDLER;

    [Space (20f)]

    [Header ("Image")]
    public SpriteRenderer spriteRenderer;
    public Sprite image_Circle;
    public Sprite image_Square;
    
    public float GetStartTime()
    {
        return startTime;
    }

    public float GetEndTime()
    {
        return endTime;
    }

    public void SetStartPosition(Vector3 position)
    {
        startPosition = position;
    }

    public void SetEndPosition(Vector3 position)
    {
        endPosition = position;
    }

    public void SetStartTime(float time)
    {
        startTime = time;
    }

    public void SetEndTime(float time)
    {
        endTime = time;
    }

    public void SetNodeID(int fId, int tId)
    {
        startNodeID = fId;
        endNodeID = tId;
    }

    public void SetTimeHandler(TimeHandler handler)
    {
        TIME_HANDLER = handler;
    }

    public void SetPacket(Vector3 start_p, Vector3 end_p, float start_t, float end_t, int fId, int tId, TimeHandler handler)
    {
        startPosition = start_p;
        endPosition = end_p;
        startTime = start_t;
        endTime = end_t;
        startNodeID = fId;
        endNodeID = tId;
        TIME_HANDLER = handler;
    }

    public void SetImage(int imageIndex)
    {
        switch(imageIndex)
        {
            case 0:
                spriteRenderer.sprite = image_Circle;
                break;
            case 1:
                spriteRenderer.sprite = image_Square;
                break;
            default:
                break;
        }
    }
    void Start()
    {
        duration = endTime-startTime;
    }


    // Update is called once per frame
    void Update()
    {
        currentTime = TIME_HANDLER.GetCurrentTime();
        if(currentTime >= startTime && currentTime <= endTime)
        {
            movePacket();
        }
        else if(endTime < currentTime)
        {
            gameObject.SetActive(false);
        }
    }

    void movePacket()
    {
        // Calculate the normalized time (0 to 1) based on the current time and duration
        float normalizedTime = (currentTime - startTime) / duration;

        // Update the object's position based on the starting and ending positions
        transform.position = Vector3.Lerp(startPosition, endPosition, normalizedTime);
    }
}
