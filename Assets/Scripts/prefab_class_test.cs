using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI; 
using System.IO;

public class prefab_class_test : MonoBehaviour
{

    public SatObject satPrefab;
    public GwObject gwPrefab;
    public UeObject uePrefab;
    public SvObject svPrefab;
    public PacketObject forwardPacketPrefab;
    public PacketObject backwardPacketPrefab;
    // public PacketObject1 PacketPrefab;
    public Slider timeScalar;
    public Slider currentTimeBar;

    List<int> gw_id = new List<int>(new int[] {633});
    List<int> sat_id = new List<int>(new int[] {342,123,520});
    List<int> ue_id = new List<int>(new int[] {632});
    List<int> sv_id = new List<int>(new int[] {634});

    Dictionary<int,SatObject> dictOfsat = new Dictionary<int,SatObject>();
    Dictionary<int,GwObject> dictOfgw = new Dictionary<int,GwObject>();
    Dictionary<int,UeObject> dictOfue = new Dictionary<int,UeObject>();
    Dictionary<int,SvObject> dictOfsv = new Dictionary<int,SvObject>();

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

        string xmlfilePath = "C:/FOR_ASSIGNMENT/log/uplink_30s.xml";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlfilePath);

        timeScalar.value = 0.0f;
        timeScalar.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);

        dictOfloc.Add(632,new Vector3(-350f,-150f));
        dictOfloc.Add(633,new Vector3(300f,-150f));
        dictOfloc.Add(634,new Vector3(400f,-150f));
        dictOfloc.Add(342,new Vector3(-150f,100f));
        dictOfloc.Add(123,new Vector3(0f,150f));
        dictOfloc.Add(520,new Vector3(150f,100f));

        string tagName = "node";
        XmlNodeList nodes = xmlDoc.GetElementsByTagName(tagName);

        foreach (XmlNode targetNode in nodes)
        {
            int node_id = int.Parse(targetNode.Attributes["id"].Value);
            float locX = float.Parse(targetNode.Attributes["locX"].Value);
            float locY = float.Parse(targetNode.Attributes["locY"].Value);
            Vector3 position = new Vector3(); 
            //position = new Vector3(locX, -locY);
            

            if(sat_id.Contains(node_id))
            {
                position = dictOfloc[node_id];
                SatObject satObject = Instantiate(satPrefab,position,Quaternion.identity,GameObject.Find("Canvas").transform);//, position, Quaternion.identity, GameObject.Find("Canvas").transform);
                satObject.node_id = node_id;
                satObject.position = position;
                dictOfsat.Add(node_id,satObject);
            }
            else if(gw_id.Contains(node_id))
            {
                position = dictOfloc[node_id];
                GwObject gwObject = Instantiate(gwPrefab,position,Quaternion.identity,GameObject.Find("Canvas").transform);//, position, Quaternion.identity, GameObject.Find("Canvas").transform);
                gwObject.node_id = node_id;
                gwObject.position = position;
                dictOfgw.Add(node_id,gwObject);
            }
            else if(ue_id.Contains(node_id))
            {
                position = dictOfloc[node_id];
                UeObject ueObject = Instantiate(uePrefab,position,Quaternion.identity,GameObject.Find("Canvas").transform);//, position, Quaternion.identity, GameObject.Find("Canvas").transform);
                ueObject.node_id = node_id;
                ueObject.position = position;
                dictOfue.Add(node_id,ueObject);
            }
            else if(sv_id.Contains(node_id))
            {
                position = dictOfloc[node_id];
                SvObject svObject = Instantiate(svPrefab,position,Quaternion.identity,GameObject.Find("Canvas").transform);//, position, Quaternion.identity, GameObject.Find("Canvas").transform);
                svObject.node_id = node_id;
                svObject.position = position;
                dictOfsv.Add(node_id,svObject);
            }
        }

        // PACKET PROCESSING
        tagName = "p";
        XmlNodeList packets = xmlDoc.GetElementsByTagName(tagName);
        int Count = 0;
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
            // PacketObject1 packet;

            if(sat_id.Contains(fId))
                start_Position = dictOfsat[fId].position;
            else if(gw_id.Contains(fId))
                start_Position = dictOfgw[fId].position;
            else if(ue_id.Contains(fId))
                start_Position = dictOfue[fId].position;
            else if(sv_id.Contains(fId))
                start_Position = dictOfsv[fId].position;


            if(sat_id.Contains(tId))
                end_Position = dictOfsat[tId].position;
            else if(gw_id.Contains(tId))
                end_Position = dictOfgw[tId].position;
            else if(ue_id.Contains(tId))
                end_Position = dictOfue[tId].position;
            else if(sv_id.Contains(tId))
                end_Position = dictOfsv[tId].position;

            Vector3 directionVector = end_Position - start_Position;
            Vector3 normVector = new Vector3(-directionVector.y,directionVector.x).normalized;

            if((fId==632&&tId==342)||(fId==342&&tId==123)||(fId==123&&tId==520)||(fId==520&&tId==633)||(fId==633&&tId==634))
            {
                packet = Instantiate(forwardPacketPrefab,GameObject.Find("Canvas").transform);
                parallel_Position = 0*normVector;
                packet.start_Position = start_Position+parallel_Position;
                packet.end_Position = end_Position+parallel_Position;
                packet.start_nodeID = fId;
                packet.end_nodeID = tId;
                packet.start_Time = fbTx;
                packet.end_Time = lbRx;
                packet.duration = lbRx - fbTx;

            }
            else if((tId==632&&fId==342)||(tId==342&&fId==123)||(tId==123&&fId==520)||(tId==520&&fId==633)||(tId==633&&fId==634))
            {
                packet = Instantiate(backwardPacketPrefab,GameObject.Find("Canvas").transform);
                parallel_Position = normVector*(-15);
                packet.start_Position = start_Position+parallel_Position;
                packet.end_Position = end_Position+parallel_Position;
                packet.start_nodeID = fId;
                packet.end_nodeID = tId;
                packet.start_Time = fbTx;
                packet.end_Time = lbRx;
                packet.duration = lbRx - fbTx;
            }

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

            if(c == 2) //TxBuffer
            {
                if(sat_id.Contains(id))
                {
                    dictOfsat[id].AddTx(t,v);
                }
                else if(gw_id.Contains(id)) 
                {
                    dictOfgw[id].AddTx(t,v);
                }
                else if(ue_id.Contains(id))
                {
                    dictOfue[id].AddTx(t,v);
                }
                else if(sv_id.Contains(id))
                {
                    dictOfsv[id].AddTx(t,v);
                }
            }
            else if( c == 1) //RxBuffer
            {
                if(sat_id.Contains(id))
                {
                    SatObject nodeObject = dictOfsat[id];
                    nodeObject.AddRx(t,v);
                }
                else if(gw_id.Contains(id)) 
                {
                    GwObject nodeObject = dictOfgw[id];
                    nodeObject.AddRx(t,v);
                }      
                else if(ue_id.Contains(id))
                {
                    dictOfue[id].AddRx(t,v);
                }
                else if(sv_id.Contains(id))
                {
                    dictOfsv[id].AddRx(t,v);
                }      
            }
            else if( c == 3) //CW change
            {
                if(sat_id.Contains(id))
                {
                    SatObject nodeObject = dictOfsat[id];
                    nodeObject.AddCw(t,v);
                }
                else if(gw_id.Contains(id)) 
                {
                    GwObject nodeObject = dictOfgw[id];
                    nodeObject.AddCw(t,v);
                }  
                else if(ue_id.Contains(id))
                {
                    dictOfue[id].AddCw(t,v);
                }
                else if(sv_id.Contains(id))
                {
                    dictOfsv[id].AddCw(t,v);
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