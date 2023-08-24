using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI; 
using System.IO;

public class PacingTest : MonoBehaviour
{
  public ComObject ComPrefab;
    public RtObject RtPrefab;
    public PacketObject forwardPacketPrefab;
    public PacketObject backwardPacketPrefab;
    public Slider timeScalar;
    public Slider currentTimeBar;

    List<int> Com_id = new List<int>(new int[] {0,1,4,5});
    List<int> Rt_id = new List<int>(new int[] {2,3});

    Dictionary<int,ComObject> dictOfcom = new Dictionary<int,ComObject>();
    Dictionary<int,RtObject> dictOfrt = new Dictionary<int,RtObject>();

    Dictionary<int,Vector3> dictOfloc = new Dictionary<int,Vector3>();


    float sim_End_Time = 0f;

    float current_Time= 0f;

    public Text timeText;

    /*load time trace*/
    private float sceneLoadStartTime;
    private string logFilePath = "LoadTimeLog.txt";
    private int writable = 0;

    void Awake()
    {
        sceneLoadStartTime = Time.realtimeSinceStartup;
    }
    


    void Start()
    {

        string xmlfilePath = "C:/FOR_ASSIGNMENT/NS3VIS/load_time_test_with_0 info.xml";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlfilePath);

        timeScalar.value = 0.0f;
        timeScalar.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);

        dictOfloc.Add(0,new Vector3(-250f,150f));
        dictOfloc.Add(1,new Vector3(-250f,-150f));
        dictOfloc.Add(4,new Vector3(250f,150f));
        dictOfloc.Add(5,new Vector3(250f,-150f));
        dictOfloc.Add(2,new Vector3(-250f,0f));
        dictOfloc.Add(3,new Vector3(250f,0f));

        string tagName = "node";
        XmlNodeList nodes = xmlDoc.GetElementsByTagName(tagName);

        foreach (XmlNode targetNode in nodes)
        {
            int node_id = int.Parse(targetNode.Attributes["id"].Value);
            float locX = float.Parse(targetNode.Attributes["locX"].Value);
            float locY = float.Parse(targetNode.Attributes["locY"].Value);
            Vector3 position = new Vector3(); 
            //position = new Vector3(locX, -locY);
            

            if(Com_id.Contains(node_id))
            {
                position = dictOfloc[node_id];
                ComObject comObject = Instantiate(ComPrefab,position,Quaternion.identity,GameObject.Find("Canvas").transform);//, position, Quaternion.identity, GameObject.Find("Canvas").transform);
                comObject.node_id = node_id;
                comObject.position = position;
                dictOfcom.Add(node_id,comObject);
            }
            else if(Rt_id.Contains(node_id))
            {
                position = dictOfloc[node_id];
                RtObject rtObject = Instantiate(RtPrefab,position,Quaternion.identity,GameObject.Find("Canvas").transform);//, position, Quaternion.identity, GameObject.Find("Canvas").transform);
                rtObject.node_id = node_id;
                rtObject.position = position;
                dictOfrt.Add(node_id,rtObject);
            }
        }

        // PACKET PROCESSING
        tagName = "p";
        XmlNodeList packets = xmlDoc.GetElementsByTagName(tagName);

        foreach(XmlNode targetPacket in packets)
        {
            int fId = int.Parse(targetPacket.Attributes["fId"].Value);
            int tId = int.Parse(targetPacket.Attributes["tId"].Value);
            float fbTx = float.Parse(targetPacket.Attributes["fbTx"].Value);
            float lbRx = float.Parse(targetPacket.Attributes["lbRx"].Value);
            Vector3 start_Position = new Vector3(0,0);
            Vector3 end_Position = new Vector3(0,0);
            Vector3 parallel_Position = new Vector3();
            PacketObject packet=new PacketObject();
            
            if(Com_id.Contains(fId))
                start_Position = dictOfcom[fId].position;
            else if(Rt_id.Contains(fId))
                start_Position = dictOfrt[fId].position;

            if(Com_id.Contains(tId))
                end_Position = dictOfcom[tId].position;
            else if(Rt_id.Contains(tId))
                end_Position = dictOfrt[tId].position;
            
            Vector3 directionVector = end_Position - start_Position;
            Vector3 normVector = new Vector3(-directionVector.y,directionVector.x).normalized;

            if(fId == 0 || fId == 1 || (fId == 2 && tId == 3) || (fId == 3 && (tId == 4 || tId ==5)))
            {
                packet = Instantiate(forwardPacketPrefab,GameObject.Find("Canvas").transform);
                parallel_Position = 0*normVector;
            }
            else
            {
                packet = Instantiate(backwardPacketPrefab,GameObject.Find("Canvas").transform);
                parallel_Position = normVector*(-15);
            }
            packet.start_Position = start_Position+parallel_Position;
            packet.end_Position = end_Position+parallel_Position;
            packet.start_Time = fbTx;
            packet.end_Time = lbRx;
            packet.duration = lbRx - fbTx;

            if(sim_End_Time<packet.end_Time)
            {
                sim_End_Time = packet.end_Time;
            }
        }

        // BUFFER PROCESSING
        tagName = "nc";
        XmlNodeList buffer_sizes = xmlDoc.GetElementsByTagName(tagName);

        foreach(XmlNode targetBuffer in buffer_sizes)
        {
            int c = int.Parse(targetBuffer.Attributes["c"].Value);
            int id = int.Parse(targetBuffer.Attributes["i"].Value);
            float t = float.Parse(targetBuffer.Attributes["t"].Value);
            int v = int.Parse(targetBuffer.Attributes["v"].Value);

            if(c == 5) //TxBuffer
            {
                if(Com_id.Contains(id))
                {
                    ComObject nodeObject = dictOfcom[id];
                    nodeObject.AddTx(t,v);
                }
                // else if(Rt_id.Contains(id)) 
                // {
                //     RtObject nodeObject = dictOfrt[id];
                //     nodeObject.AddTx(t,v);
                // }
            }
            else if( c == 4) //RxBuffer
            {
                if(Com_id.Contains(id))
                {
                    ComObject nodeObject = dictOfcom[id];
                    nodeObject.AddRx(t,v);
                }
                // else if(Rt_id.Contains(id)) 
                // {
                //     RtObject nodeObject = dictOfrt[id];
                //     nodeObject.AddRx(t,v);
                // }            
            }
            else if( c == 6) //CW change
            {
                if(Com_id.Contains(id))
                {
                    ComObject nodeObject = dictOfcom[id];
                    nodeObject.AddCw(t,v);
                }
            }
            else if( c == 1)
            {
                if(Rt_id.Contains(id))
                {
                    dictOfrt[id].EnQueue(t,v);
                }
            }
            else if( c == 2)
            {
                if(Rt_id.Contains(id))
                {
                    dictOfrt[id].DeQueue(t,v);
                }
            }
            if(sim_End_Time<t)
                sim_End_Time = t;
        }
    }

    void Update()
    {
        if(current_Time<sim_End_Time)
        {
            current_Time += Time.deltaTime;
            timeText.text = current_Time.ToString();
            Time.timeScale = ((timeScalar.value)/500000);

            currentTimeBar.value = current_Time/sim_End_Time;
        }
        /*load time trace start*/
         if(writable == 0)
        {
            float loadTime = Time.realtimeSinceStartup - sceneLoadStartTime;
            writable = 1;
            WriteToLogFile("load time: " + loadTime.ToString("F6") + "'s");
        }
        /* end */

    }
    /*load time trace start */
    private void WriteToLogFile(string log)
    {
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine(log);
        }
    }
    /* end */

    
}
