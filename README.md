# UVNS
UVNS is Unity-based Visualizer for ns-3. 

## Implement
To use UVNS, you have to install Unity first. After install Unity, download above all files and open the Unity Hub. Choose 'Open' - 'Add project from disk' and select folder which storing above files. Wait a moment and the Unity project will start. To see your own simulation, you need to modify some of the scripts below.

## Scripts
### XMLParser.cs
You need to set the path to the xml file('xmlfilePath') that stores the log of your simulation. You also need to set list of nodes which your simulation use, group by category like satellite, gateway, server, etc.

You can set 'useOriginalPosition'. If this value is true, the node's position is set to the node position written in xml file, and if false, you need to set your own position.

### NodeObject.cs and PacketObject.cs
You don't necessarily have to edit this file, but if you want to arrange the variables or modify the visiblity of information, you can adjust the visibility of the inspector window with a few settings.

You can choose this settings:
- public variables: You can see this variable in inspector window. If you do not want to see, delete 'public'.
- [Header ("")]: You can categorizing the variables
- [Space ()]: Just spacing in inspector window

