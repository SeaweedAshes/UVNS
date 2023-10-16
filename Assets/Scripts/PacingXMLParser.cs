using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI; 
using System.IO;
using UnityEditor;

public class PacingXMLParser : MonoBehaviour
{
    // You must set the following in the inspector window
    public NodeObject NodePrefab;
    public PacketObject PacketPrefab;
    public RtObject RtPrefab;
    public TimeHandler TIME_HANDLER;
    public PacketControler PACKET_CONTROLER;
  
    // Set to true if using the position information in the XML file, otherwise set to false.
    bool useOriginalPosition = false;

    // ID of nodes
    List<int> IdOfCom = new List<int>(new int[] {0,1,4,5});
    List<int> IdOfRt = new List<int>(new int[] {2,3});

    // Dictionary that maps ID to NodeObject
    Dictionary<int, NodeObject> dictOfNode = new Dictionary<int, NodeObject>();
    // Dictionary that maps ID to RtObject
    Dictionary<int, RtObject> dictOfRt = new Dictionary<int, RtObject>();

    // Dictionary that maps ID to position
    Dictionary<int,Vector3> dictOfLoc = new Dictionary<int,Vector3>();

    // List of PacketObject
    List<PacketObject> listOfPacket = new List<PacketObject>();


    // Awake is called before the Start
    void Awake()
    {
        Init_location();

        string xmlfilePath = "Assets/XMLDocument/tcp_pacing_log.xml";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlfilePath);
        
        // Process node tag
        string tagName = "node";
        XmlNodeList nodes = xmlDoc.GetElementsByTagName(tagName);
        foreach (XmlNode targetNode in nodes)
        {
            int nodeId = int.Parse(targetNode.Attributes["id"].Value);
            float locX = float.Parse(targetNode.Attributes["locX"].Value);
            float locY = float.Parse(targetNode.Attributes["locY"].Value);
            
            Process_node_tag(nodeId, locX, locY);
        }

        // Process p tag
        tagName = "p";
        XmlNodeList packets = xmlDoc.GetElementsByTagName(tagName);
        foreach(XmlNode targetPacket in packets)
        {
            int fId = int.Parse(targetPacket.Attributes["fId"].Value);
            int tId = int.Parse(targetPacket.Attributes["tId"].Value);
            float fbTx = float.Parse(targetPacket.Attributes["fbTx"].Value);
            float lbRx = float.Parse(targetPacket.Attributes["lbRx"].Value);

            Process_p_tag(fId,tId,fbTx,lbRx);
        }

        // Set packet array and TimeHandler of PacketControler
        PACKET_CONTROLER.SetArray(listOfPacket.ToArray());
        PACKET_CONTROLER.SetTimeHandler(TIME_HANDLER);

        // Process nc tag
        tagName = "nc";
        XmlNodeList bufferSizes = xmlDoc.GetElementsByTagName(tagName);

        foreach(XmlNode targetBuffer in bufferSizes)
        {
            int c = int.Parse(targetBuffer.Attributes["c"].Value);
            int id = int.Parse(targetBuffer.Attributes["i"].Value);
            float t = float.Parse(targetBuffer.Attributes["t"].Value);
            int v = int.Parse(targetBuffer.Attributes["v"].Value);

            Process_nc_tag(c, id, t, v);  
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Which object is selected
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast (pos, Vector2.zero, 0f);

            if( hit.collider != null)
            {
                Selection.activeGameObject = hit.collider.gameObject;
            }
        }
    }

    
    
    void Init_location()
    {
        if(!useOriginalPosition)
        {
            dictOfLoc.Add(0,new Vector3(-250f,150f));
            dictOfLoc.Add(1,new Vector3(-250f,-150f));
            dictOfLoc.Add(4,new Vector3(250f,150f));
            dictOfLoc.Add(5,new Vector3(250f,-150f));
            dictOfLoc.Add(2,new Vector3(-250f,0f));
            dictOfLoc.Add(3,new Vector3(250f,0f));
        }
    }

    
    void Process_node_tag(int nodeId, float locX, float locY)
    { 
        Vector3 position;

        // Check nodeId whether the node is need to show
        if(IdOfCom.Contains(nodeId))
            {
                if(useOriginalPosition)
                    position = new Vector3(locX, locY, 0.0f);
                else 
                    position = dictOfLoc[nodeId];
                
                // Create NodeInstance from NodePrefab
                NodeObject NodeInstance = Instantiate(NodePrefab, position, Quaternion.identity, this.transform);
                NodeInstance.SetNodeID(nodeId);
                NodeInstance.SetPosition(position);
                NodeInstance.SetTimeHandler(TIME_HANDLER);
                dictOfNode.Add(nodeId, NodeInstance);
                
                // Set Image according nodeId
                NodeInstance.SetImage(5);
            }
        else if(IdOfRt.Contains(nodeId))
        {
            if(useOriginalPosition)
                position = new Vector3(locX,locY,0.0f);
            else
                position = dictOfLoc[nodeId];
            
            // Create RtInstance from RtPrefab
            RtObject RtInstance = Instantiate(RtPrefab, position ,Quaternion.identity, this.transform);
            RtInstance.SetNodeID(nodeId);
            RtInstance.SetPosition(position);
            RtInstance.SetTimeHandler(TIME_HANDLER);
            dictOfRt.Add(nodeId, RtInstance);
        }
    }
    
    void Process_p_tag(int fId, int tId, float fbTx, float lbRx)
    {
        Vector3 startPosition=new Vector3();
        Vector3 endPosition=new Vector3();
        Vector3 translation = new Vector3();

        // Create PacketInstance from PacketPrefab
        PacketObject PacketInstance = Instantiate(PacketPrefab,this.transform);

        // Set startPosition to position of node with fId
        if(IdOfCom.Contains(fId))
                startPosition = dictOfNode[fId].position;
        else if(IdOfRt.Contains(fId))
                startPosition = dictOfRt[fId].position;
            
        // Set endPosition to position of node with tId
        if(IdOfCom.Contains(tId))
                endPosition = dictOfNode[tId].position;
        else if(IdOfRt.Contains(tId))
                endPosition = dictOfRt[tId].position;

        // Calculate translation according packet direction
        Vector3 directionVector = endPosition - startPosition;
        Vector3 normVector = new Vector3(-directionVector.y,directionVector.x).normalized;

        if(fId == 0 || fId == 1 || (fId == 2 && tId == 3) || (fId == 3 && (tId == 4 || tId ==5)))
        {
            PacketInstance.SetImage(0);
            translation = 0*normVector;
        }
        else
        {
            PacketInstance.SetImage(1);
            translation = normVector*(-15);
        }

        // Set position, time and TIME_HANDLER of PacketInstance
        PacketInstance.transform.position = new Vector2(1000f,1000f);
        PacketInstance.SetPacket(startPosition+translation, endPosition+translation, fbTx, lbRx, fId, tId, TIME_HANDLER);
        PacketInstance.gameObject.SetActive(false);
        listOfPacket.Add(PacketInstance);

        if(TIME_HANDLER.GetEndTime()<lbRx)
        {
            TIME_HANDLER.SetEndTime(lbRx);
        }
    }

    void Process_nc_tag(int c, int id, float t, int v)
    {
        // Check id whether the node counter is processed
          if(dictOfNode.ContainsKey(id))
            {
                switch(c)
                {
                    case 5: //Txbuffer
                        dictOfNode[id].AddTx(t, v);
                        break;
                    
                    case 4: //Rxbuffer
                        dictOfNode[id].AddRx(t, v);
                        break;

                    case 6: //CongestionWindow
                        dictOfNode[id].AddCw(t, v);
                        break;
                    default:
                        break;
                }
            }
            else if(dictOfRt.ContainsKey(id))
            {
                switch(c)
                {
                    case 1: //Enqueue
                        dictOfRt[id].AddEnqueue(t,v);
                        break;
                    case 2: //Dequeue
                        dictOfRt[id].AddDequeue(t,v);
                        break;
                    default:
                        break;
                }
            }
            if(TIME_HANDLER.GetEndTime()<t)
                TIME_HANDLER.SetEndTime(t);
    }

}
