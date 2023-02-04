using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public int maxRows, maxColumns; //max number of rows and columns the map grid can have
    private Room[,] roomGrid; //stores data-only objects that hold a rooms required # and relative positIons of entrances

    
    
    // Start is called before the first frame update
    void Start()
    {
        roomGrid = new Room[maxRows, maxColumns];

        GenerateRooms();
        DebugPrintGrid();

    }

    /// <summary>
    /// The function for generating the room layout for the map
    /// </summary>
    private void GenerateRooms()
    {
        List<Room> generatedRooms = new List<Room>(); //holds rooms that are created
        List<int[]> generatedIndices = new List<int[]>(); //holds grid positions of rooms that are created

        int[] gridPosition = new int[2]; //holds grid position for a new room
        int num; //holds generated random number for each possible room space

        //iterate through 2D array, generating Room objects in random spots
        for(int i = 0; i < maxRows; i++)
        {
            for(int j = 0; j < maxColumns; j++)
            {
                //check for rooms in square above. Don't generate 2 rooms next to each other
                if (i <= 0 || roomGrid[i - 1, j] == null)
                {
                    num = Random.Range(0, 2); //generates a 0 or 1

                    //generate a room if the number is a 1 
                    if (num > 0)
                    {
                        roomGrid[i, j] = new Room(); //new room with no details
                        generatedRooms.Add(roomGrid[i, j]); //add the room to the list
                        gridPosition[0] = i;
                        gridPosition[1] = j;
                        generatedIndices.Add(gridPosition); //add the room's position to list

                        if((i == 0 || i == maxRows - 1) && (j == 0 || j == maxColumns - 1)) //room is at a corner of the map
                        {
                            Debug.Log("room [" + i + "," + j + "] corner");
                            roomGrid[i, j].numEntrances = Random.Range(1, 3); //corner rooms can have 1-2 entrances
                        }
                        else if((i == 0 || i == maxRows - 1) || (j == 0 || j == maxColumns - 1)) //room is at an edge of the map, but not a corner
                        {
                            Debug.Log("room [" + i + "," + j + "] edge");
                            roomGrid[i, j].numEntrances = Random.Range(1, 4); //edge rooms can have 1-3 entrances
                        }
                        else //room isn't on an edge
                        {
                            Debug.Log("room [" + i + "," + j + "] not edge");
                            roomGrid[i, j].numEntrances = Random.Range(1, 5); //interior rooms can have up to 4 entrances

                        }

                        Debug.Log("room [" + i + "," + j + "] " + roomGrid[i, j].numEntrances + " entrances");
                        j++; //iterate j an extra time to ensure 2 rooms aren't generated next to each other
                    }
                }
            }
        }

        SetEntranceDirections(generatedRooms.ToArray(), generatedIndices.ToArray());
    }

    /// <summary>
    /// Pick directions for entrances of each room to exist in
    /// </summary>
    /// <param name="rooms">rooms we need to define entrance directions for</param>
    /// <param name="">the grid indices of each passed room</param>
    private void SetEntranceDirections(Room[] rooms, int[][] indices)
    {
        int edges = 0; //number of edges a room is on, up to 2

        //iterate through all the passed rooms
        for(int i = 0; i < rooms.Length; i++)
        {
            //set all direction bools to true first, makes it easier to keep track of edges that are found
            for(int j = 0; j < 4; j++)
            {
                rooms[i].entranceDirs[j] = true;
            }

            //set bools for directions on edges to false, indices syntax: indices[i][0] = column number of room i, indices[i][1] = row number of room i
            if (indices[i][1] == 0) //north edge
            {
                Debug.Log("north edge room at [" + indices[i][0] + ", " + indices[i][1] + "]");
                rooms[i].entranceDirs[0] = false;
                edges++;
            }
            else if (indices[i][1] == maxRows - 1) //south edge
            {
                Debug.Log("south edge room at " + indices[i][0] + ", " + indices[i][1] + "]");
                rooms[i].entranceDirs[1] = false;
                edges++;
            }
            if (indices[i][0] == maxColumns - 1) //east edge
            {
                Debug.Log("east edge room at " + indices[i][0] + ", " + indices[i][1] + "]");
                rooms[i].entranceDirs[2] = false;
                edges++;
            }
            else if (indices[i][1] == maxRows - 1) //west edge
            {
                Debug.Log("west edge room at " + indices[i][0] + ", " + indices[i][1] + "]");
                rooms[i].entranceDirs[3] = false;
                edges++;
            }

            switch (rooms[i].numEntrances)
            {
                case 4: //4 entrances, all direction bools stay true
                    break;

                case 3: //3 entrances, one direction = false
                    if (edges == 0) //if the room is not on an edge, a direction hasn't been ruled out already
                    {
                        rooms[i].entranceDirs[Random.Range(0, 4)] = false; //set one random direction to false
                    }  
                    break;

                case 2: //2 entrances, 2 directions = false
                    int index; //holds randomly generated index
                    if (edges == 1) //room is on edge but not corner, need to rule out 1 more direction
                    {
                        //get random direction indices until you get a direction that isn't already false
                        index= Random.Range(0, 4); 
                        while (rooms[i].entranceDirs[index] == false)
                        {
                            index = Random.Range(0, 4);
                        }

                        rooms[i].entranceDirs[index] = false; //set another direction to false
                    }
                    else if (edges == 0) //room isn't on edge, need to rule out 2 directions
                    {
                        index = Random.Range(0, 4); //get a random index
                        rooms[i].entranceDirs[index] = false; //rule out the first random direction

                        //get a different direction to rule out
                        int index2 = Random.Range(0, 4); 
                        while(index2 == index)
                        {
                            index2 = Random.Range(0, 4);
                        }
                        rooms[i].entranceDirs[index2] = false;
                    }
                    break;

                case 1: //1 entrance, only 1 direction != false
                    int dir;
                    if(edges == 2) //room on corner. Need to rule out 1 more direction
                    {
                        //get random direction until you find one that is still true
                        dir = Random.Range(0, 4);
                        while(rooms[i].entranceDirs[dir] == false)
                        {
                            dir = Random.Range(0, 4);
                        }
                        rooms[i].entranceDirs[dir] = false; //set 3rd direction to false
                    }
                    else if(edges == 1) //room on edge. need to rule out 2 more directions
                    {
                        dir = Random.Range(0, 4); //get a random index
                        rooms[i].entranceDirs[dir] = false; //rule out the first random direction

                        //get a different direction to rule out
                        int index2 = Random.Range(0, 4);
                        while (index2 == dir)
                        {
                            index2 = Random.Range(0, 4);
                        }
                        rooms[i].entranceDirs[index2] = false;
                    }
                    else //room not on edge, need to pick 1 direction for entrance
                    {
                        dir = Random.Range(0, 4); //pick random direction for entrance

                        //set all other directions to false
                        for(int x = 0; x < 4; x++)
                        {
                            if(x != dir)
                            {
                                rooms[i].entranceDirs[x] = false;
                            }
                        }
                    }
                    break;

                default:
                    Debug.Log("Invalid number of entrances");
                    break;
            }
        }
    }


    /// <summary>
    /// small debug function to print the current state of the room/map grid
    /// </summary>
    private void DebugPrintGrid()
    {
        string printStr = "";

        for(int i = 0; i < maxRows; i++)
        {
            for(int j = 0; j < maxColumns; j++)
            {
                if (roomGrid[i, j] != null)
                {
                    printStr += " " + roomGrid[i, j].numEntrances + "["; //print number of entrances if there is a room here
                    for(int k = 0; k < 4; k++)
                        printStr += roomGrid[i,j].entranceDirs[k] + ",";
                    printStr += "]";
                }
                else
                {
                    printStr += " " + 0; //print a 0 if nothing is here
                }
            }

            printStr += "\n";
        }

        Debug.Log(printStr);
    }
}
