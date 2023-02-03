using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room 
{

    public int numEntrances; //how many entrances this room has

    //holds values for whether or not there is an entrance in each direction
    public bool north, south, east, west;

    public bool hallway; //whether this is a hallway, false = this is a room
}
