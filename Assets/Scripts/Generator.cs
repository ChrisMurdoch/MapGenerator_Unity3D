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

                        if((i == 0 || i == maxRows - 1) && (j == 0 || j == maxColumns - 1)) //room is at a corner of the map
                        {
                            roomGrid[i, j].numEntrances = Random.Range(1, 3); //corner rooms can have 1-2 entrances
                        }
                        else if((i == 0 || i == maxRows - 1) || (j == 0 || j == maxColumns - 1)) //room is at an edge of the map, but not a corner
                        {
                            roomGrid[i, j].numEntrances = Random.Range(1, 4); //edge rooms can have 1-3 entrances
                        }
                        else //room isn't on an edge
                        {
                            roomGrid[i, j].numEntrances = Random.Range(1, 5); //interior rooms can have up to 4 entrances

                        }

                        j++; //iterate j an extra time toensure 2 rooms aren't generated next to each other
                    }
                }
            }
        }
    }

    /// <summary>
    /// Pick directions for entrances of each room to exist in
    /// </summary>
    /// <param name="rooms">rooms we need to define entrance directions for</param>
    /// <param name="">the grid indices of each passed room</param>
    private void SetEntranceDirections(Room[] rooms, int[,][] indices)
    {
        //iterate through all the passed rooms
        for(int i = 0; i < rooms.Length; i++)
        {

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
                    printStr += " " + roomGrid[i, j].numEntrances; //print number of entrances if there is a room here
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
