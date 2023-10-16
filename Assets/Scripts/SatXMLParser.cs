using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI; 
using System.IO;
using UnityEditor;


public class SatXMLParser : MonoBehaviour
{
    // You must set the following in the inspector window
    public NodeObject NodePrefab;
    public PacketObject PacketPrefab;
    public TimeHandler TIME_HANDLER;
    public PacketControler PACKET_CONTROLER;
  
    // Set to true if using the position information in the XML file, otherwise set to false.
    bool useOriginalPosition = false;

    // ID of nodes
    List<int> IdOfGw = new List<int>(new int[] {633});
    List<int> IdOfSat = new List<int>(new int[] {342,123,520});
    List<int> IdOfUe = new List<int>(new int[] {632});
    List<int> IdOfSvr = new List<int>(new int[] {634});

    // Dictionary that maps ID to NodeObject
    Dictionary<int, NodeObject> dictOfNode = new Dictionary<int, NodeObject>();
    // Dictionary that maps ID to position
    Dictionary<int,Vector3> dictOfLoc = new Dictionary<int,Vector3>();
    
    // List of PacketObject
    List<PacketObject> listOfPacket = new List<PacketObject>();

    // Awake is called before the Start
    void Awake()
    {
        Init_location();

        string xmlfilePath = "Assets/XMLDocument/uplink_30s.xml";
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
            dictOfLoc.Add(632,new Vector3(-350f,-150f));
            dictOfLoc.Add(633,new Vector3(300f,-150f));
            dictOfLoc.Add(634,new Vector3(400f,-150f));
            dictOfLoc.Add(342,new Vector3(-150f,100f));
            dictOfLoc.Add(123,new Vector3(0f,150f));
            dictOfLoc.Add(520,new Vector3(150f,100f));
        }
    }

    void Process_node_tag(int nodeId, float locX, float locY)
    { 
        Vector3 position;
        
        // Check nodeId whether the node is need to show
        if(IdOfSat.Contains(nodeId) || IdOfGw.Contains(nodeId) || IdOfUe.Contains(nodeId) || IdOfSvr.Contains(nodeId))
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
                if(IdOfSat.Contains(nodeId)) NodeInstance.SetImage(2);
                if(IdOfGw.Contains(nodeId)) NodeInstance.SetImage(3);
                if(IdOfUe.Contains(nodeId)) NodeInstance.SetImage(1);
                if(IdOfSvr.Contains(nodeId)) NodeInstance.SetImage(4);
            }
    }
    
    void Process_p_tag(int fId, int tId, float fbTx, float lbRx)
    {
        // Check fId and tId whether the packet is need to show
        if(dictOfNode.ContainsKey(fId) && dictOfNode.ContainsKey(tId))
        {
            Vector3 startPosition=new Vector3();
            Vector3 endPosition=new Vector3();
            Vector3 translation = new Vector3();

            // Create PacketInstance from PacketPrefab
            PacketObject PacketInstance = Instantiate(PacketPrefab,this.transform);

            // Set startPosition to position of node with fId
            if(IdOfSat.Contains(fId) || IdOfGw.Contains(fId) || IdOfUe.Contains(fId) || IdOfSvr.Contains(fId))
                    startPosition = dictOfNode[fId].position;
            
            // Set endPosition to position of node with tId
            if(IdOfSat.Contains(tId) || IdOfGw.Contains(tId) || IdOfUe.Contains(tId) || IdOfSvr.Contains(tId))
                    endPosition = dictOfNode[tId].position;

            // Calculate translation according packet direction 
            Vector3 directionVector = endPosition - startPosition;
            Vector3 normVector = new Vector3(-directionVector.y,directionVector.x).normalized;
            if((fId==632&&tId==342)||(fId==342&&tId==123)||(fId==123&&tId==520)||(fId==520&&tId==633)||(fId==633&&tId==634))
            {
                PacketInstance.SetImage(0);
                translation = 0*normVector;
            }
            else if((tId==632&&fId==342)||(tId==342&&fId==123)||(tId==123&&fId==520)||(tId==520&&fId==633)||(tId==633&&fId==634))
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
    }

    void Process_nc_tag(int c, int id, float t, int v)
    {
        // Check id whether the node counter is processed
          if(dictOfNode.ContainsKey(id))
            {
                // What kind of node counter
                switch(c)
                {
                    case 2: // Txbuffer
                        dictOfNode[id].AddTx(t, v);
                        break;
                    
                    case 1: // Rxbuffer
                        dictOfNode[id].AddRx(t, v);
                        break;

                    case 3: // CongestionWindow
                        dictOfNode[id].AddCw(t, v);
                        break;
                    
                    default:
                        break;
                }
            }

            if(TIME_HANDLER.GetEndTime()<t)
                TIME_HANDLER.SetEndTime(t);
    }
    
}