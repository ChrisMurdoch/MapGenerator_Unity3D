using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    //prefab references to room and hallway gameobjects to instantiate
    public GameObject[] roomPrefabs;
    public GameObject[] hallPrefabs;

    public int maxRows, maxColumns; //max number of rows and columns the map grid can have
    private Room[,] roomGrid; //stores data-only objects that hold a rooms required # and relative positIons of entrances

    public float roomSize; //how many units wide/tall each "room" prefab is
    
    
    // Start is called before the first frame update
    void Start()
    {
        roomGrid = new Room[maxRows, maxColumns];

        GenerateRooms();
        DebugPrintGrid();
        InstantiateMap();

    }

    /// <summary>
    /// The function for generating the room layout for the map
    /// </summary>
    private void GenerateRooms()
    {
        List<Room> generatedRooms = new List<Room>(); //holds rooms that are created
        List<int[]> generatedIndices = new List<int[]>(); //holds grid positions of rooms that are created

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
                        roomGrid[i, j].hallway = false; //set hallway to false to indicate this is a normal room
                        generatedRooms.Add(roomGrid[i, j]); //add the room to the list
                        int[] position = { i, j };
                        generatedIndices.Add(position); //add the room's position to list

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

                        j++; //iterate j an extra time to ensure 2 rooms aren't generated next to each other
                    }
                }
            }
        }

        string printStr = "";
        foreach(Room r in generatedRooms.ToArray())
        {
            printStr += " " + r.numEntrances + " entrance room,";
        }
        Debug.Log("room array = " + printStr);
        printStr = "";
        foreach (int[] gridPos in generatedIndices.ToArray())
        {
            printStr += " [" + gridPos[0] + "," + gridPos[1] + "],";
        }
        Debug.Log(printStr);
        SetEntranceDirections(generatedRooms.ToArray(), generatedIndices.ToArray()); //set entrance directions for each room
        GenerateHalls(generatedRooms.ToArray(), generatedIndices.ToArray()); //generate hallways based on position / direction of room entrances
    }

    /// <summary>
    /// Function for generating hallways between rooms
    /// </summary>
    private void GenerateHalls(Room[] rooms, int[][] roomIndices)
    {
        List<Room> hallways = new List<Room>();
        List<int[]> hallIndices = new List<int[]>();

        int[] index = new int[2];

        //loop through generated rooms, generate hallways to connect to all the entrances
        for(int i = 0; i < rooms.Length; i++)
        {
            Debug.Log("room[" + roomIndices[i][0] + ", " + roomIndices[i][1] + "]");

            //room has north entrance
            if (rooms[i].entranceDirs[0])
            {
                index = new int[] { roomIndices[i][0] - 1, roomIndices[i][1] }; //get index for square above current room
                Debug.Log("north entrance, hall grid position = [" + index[0] + ", " + index[1] + "]");

                //grid space north of current room is empty
                if (!ListContains(hallIndices, index)) 
                {
                    Debug.Log("creating new hallway");
                    hallways.Add(AddHallWay(index[0], index[1], 1)); //create new hallway
                    hallIndices.Add(index); //add new hallways index to hallIndices
                }
                else //a hallway already exists north of room (can't be a normal room because that was checked during generation)
                {
                    Debug.Log("adding entrance to hallway [" + index[0] + ", " + index[1] + "]");
                    roomGrid[index[0], index[1]].entranceDirs[1] = true; //add south entrance to existing hallway
                    roomGrid[index[0], index[1]].numEntrances++; //increment number of entrances
                }
            }

            //room has a south entrance
            if (rooms[i].entranceDirs[1])
            {
                index = new int[] { roomIndices[i][0] + 1, roomIndices[i][1] }; //get index for square below current room
                Debug.Log("south entrance, hall grid position = [" + index[0] + ", " + index[1] + "]");

                //grid space south of current room is empty
                if (!ListContains(hallIndices, index))
                {
                    Debug.Log("creating new hallway");
                    hallways.Add(AddHallWay(index[0], index[1], 0)); //create new hallway
                    hallIndices.Add(index); //add new hallways index to hallIndices
                }
                else //a hallway already exists south of room (can't be a normal room because that was checked during generation)
                {
                    Debug.Log("adding entrance to hallway [" + index[0] + ", " + index[1] + "]");
                    roomGrid[index[0], index[1]].entranceDirs[0] = true; //add north entrance to existing hallway
                    roomGrid[index[0], index[1]].numEntrances++; //increment number of entrances
                }
            }

            //room has an east entrance
            if (rooms[i].entranceDirs[2])
            {
                index = new int[] { roomIndices[i][0], roomIndices[i][1] + 1 }; //get index for square right of current room
                Debug.Log("east entrance, hall grid position = [" + index[0] + ", " + index[1] + "]");

                //grid space east of current room is empty
                if (!ListContains(hallIndices, index))
                {
                    Debug.Log("creating new hallway");
                    hallways.Add(AddHallWay(index[0], index[1], 3)); //create new hallway
                    hallIndices.Add(index); //add new hallways index to hallIndices
                }
                else //a hallway already exists east of room (can't be a normal room because that was checked during generation)
                {
                    Debug.Log("adding entrance to hallway [" + index[0] + ", " + index[1] + "]");
                    roomGrid[index[0], index[1]].entranceDirs[3] = true; //add west entrance to existing hallway
                    roomGrid[index[0], index[1]].numEntrances++; //increment number of entrances
                }
            }

            //room has a west entrance
            if (rooms[i].entranceDirs[3])
            {
                index = new int[] { roomIndices[i][0], roomIndices[i][1] - 1 }; //get index for square left of current room
                Debug.Log("west entrance, hall grid position = [" + index[0] + ", " + index[1] + "]");

                //grid space west of current room is empty
                if (!ListContains(hallIndices, index))
                {
                    Debug.Log("creating new hallway");
                    hallways.Add(AddHallWay(index[0], index[1], 2)); //create new hallway
                    hallIndices.Add(index); //add new hallways index to hallIndices
                }
                else //a hallway already exists west of room (can't be a normal room because that was checked during generation)
                {
                    Debug.Log("adding entrance to hallway [" + index[0] + ", " + index[1] + "]");
                    roomGrid[index[0], index[1]].entranceDirs[2] = true; //add east entrance to existing hallway
                    roomGrid[index[0], index[1]].numEntrances++; //increment number of entrances
                }
            }

        }
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
            Debug.Log("room indices [" + indices[i][0] + "," + indices[i][1] + "]");
            //set bools for directions on edges to false, indices syntax: indices[i][0] = column number of room i, indices[i][1] = row number of room i
            if (indices[i][0] == 0) //north edge
            {
                Debug.Log("north edge room at [" + indices[i][0] + ", " + indices[i][1] + "]");
                rooms[i].entranceDirs[0] = false;
                edges++;
            }
            else if (indices[i][0] == maxRows - 1) //south edge
            {
                Debug.Log("south edge room at " + indices[i][0] + ", " + indices[i][1] + "]");
                rooms[i].entranceDirs[1] = false;
                edges++;
            }
            if (indices[i][1] == maxColumns - 1) //east edge
            {
                Debug.Log("east edge room at " + indices[i][0] + ", " + indices[i][1] + "]");
                rooms[i].entranceDirs[2] = false;
                edges++;
            }
            else if (indices[i][1] == 0) //west edge
            {
                Debug.Log("west edge room at " + indices[i][0] + ", " + indices[i][1] + "]");
                rooms[i].entranceDirs[3] = false;
                edges++;
            }

            switch (rooms[i].numEntrances)
            {
                case 4: //4 entrances, all direction bools stay true
                    Debug.Log("case 4");
                    break;

                case 3: //3 entrances, one direction = false
                    Debug.Log("case 3");
                    if (edges == 0) //if the room is not on an edge, a direction hasn't been ruled out already
                    {
                        rooms[i].entranceDirs[Random.Range(0, 4)] = false; //set one random direction to false
                    }  
                    break;

                case 2: //2 entrances, 2 directions = false
                    Debug.Log("case 2");
                    int index; //holds randomly generated index
                    if (edges == 1) //room is on edge but not corner, need to rule out 1 more direction
                    {
                        Debug.Log("edge room");
                        //get random direction indices until you get a direction that isn't already false
                        do
                        {
                            index = Random.Range(0, 4);
                        } while (rooms[i].entranceDirs[index] == false);

                        rooms[i].entranceDirs[index] = false; //set another direction to false
                    }
                    else if (edges == 0) //room isn't on edge, need to rule out 2 directions
                    {
                        Debug.Log("interior rooms");
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
                    Debug.Log("case 1");
                    int dir;
                    if(edges == 2) //room on corner. Need to rule out 1 more direction
                    {
                        Debug.Log("corner room");
                        //get random direction until you find one that is still true
                        do
                        {
                            dir = Random.Range(0, 4);
                        } while (rooms[i].entranceDirs[dir] == false);

                        rooms[i].entranceDirs[dir] = false; //set 3rd direction to false
                    }
                    else if(edges == 1) //room on edge. need to rule out 2 more directions
                    {
                        Debug.Log("edge room");
                        //get random direction that isn't already ruled out
                        do
                        {
                            dir = Random.Range(0, 4);
                        } while (rooms[i].entranceDirs[dir] == false);
                        rooms[i].entranceDirs[dir] = false; //rule out the first random direction

                        //get a different direction to rule out
                        int index2 = Random.Range(0, 4);
                        while (index2 == dir || rooms[i].entranceDirs[index2] == false)
                        {
                            index2 = Random.Range(0, 4);
                        }
                        rooms[i].entranceDirs[index2] = false; //rule out second random direction
                    }
                    else //room not on edge, need to pick 1 direction for entrance
                    {
                        Debug.Log("interior room");
                        dir = Random.Range(0, 4); //pick random direction for entrance
                        Debug.Log("entrance dir index = " + dir);
                        string printStr = "directions = [";
                        //set all other directions to false
                        for(int x = 0; x < 4; x++)
                        {
                            if(x != dir)
                            {
                                rooms[i].entranceDirs[x] = false;
                            }
                            printStr += " " + x + ",";

                        }
                        Debug.Log(printStr + "]");
                    }
                    break;

                default:
                    Debug.Log("Invalid number of entrances");
                    break;
            }

            edges = 0; //reset number of edges the room has
        }
    }

    /// <summary>
    /// Add a hallway at the specified grid row/column with an entrance in the specified direction
    /// </summary>
    /// <param name="row">roomGrid row number</param>
    /// <param name="column">roomGrid column number</param>
    /// <param name="entranceDirection">direction for new hallway's entrance</param>
    /// <returns></returns>
    private Room AddHallWay(int row, int column, int entranceDirection)
    {
        //create the new hallway
        Room newHall = new Room();
        newHall.hallway = true;

        newHall.numEntrances = 1; //hallway has 1 entrance so far
        newHall.entranceDirs[entranceDirection] = true; //set new hallway to have a south entrance to match the room's north entrance
        roomGrid[row, column] = newHall; //add the hallway to the correct position on the room grid
        return newHall; //return newly created room
    }

    /// <summary>
    /// instantiate room prefabs based on the objects in the grid
    /// </summary>
    private void InstantiateMap()
    {
        Vector3 currentPos = new Vector3(0, 0, 0); //keep track of the physical position to instantiate rooms in
        float rotation = 0f; //how much to rotate the room prefab based on the entrance directions

        //loop through the grid, and instantiate rooms & hallways
        for(int i = 0; i < maxRows; i++)
        {
            for(int j = 0; j < maxColumns; j++)
            {
                //make sure there is a room in this space
                if (roomGrid[i, j] != null)
                {
                    //room has 4 entrances, rotation never needed
                    if (roomGrid[i, j].numEntrances == 4)
                    {
                        rotation = 0f;

                        //instantiate 4-entrance hallway prefab
                        if (roomGrid[i, j].hallway)
                        {
                            Instantiate(hallPrefabs[4], currentPos, Quaternion.Euler(0f, rotation, 0f));
                        }

                        //instantiate 4-entrance room prefab
                        else
                        {
                            Instantiate(roomPrefabs[4], currentPos, Quaternion.Euler(0f, rotation, 0f));
                        }
                    }

                    //room has 3 entrances, rotate based on which wall isn't an entrance, default is N, S, E
                    else if (roomGrid[i, j].numEntrances == 3)
                    {
                        //entrances are S, E, W
                        if (!roomGrid[i, j].entranceDirs[0])
                        {
                            rotation = 90f;
                        }
                        //entrances are N, E, W
                        else if (!roomGrid[i, j].entranceDirs[1])
                        {
                            rotation = 270f;
                        }
                        //entrances are N, S, W
                        else if (!roomGrid[i, j].entranceDirs[2])
                        {
                            rotation = 180f;
                        }
                        //entrances are N, S, E
                        else
                        {
                            rotation = 0f;
                        }

                        //instantiate 4-entrance hallway
                        if (roomGrid[i, j].hallway)
                        {
                            Instantiate(hallPrefabs[3], currentPos, Quaternion.Euler(0f, rotation, 0f));
                        }
                        //instantiate 4-entrance room
                        else
                        {
                            Instantiate(roomPrefabs[3], currentPos, Quaternion.Euler(0f, rotation, 0f));
                        }
                    }

                    //room has 2 entrances, choose prefab & rotate based on where entrances are, straightDefault = NS, cornerDefault = NE
                    else if (roomGrid[i, j].numEntrances == 2)
                    {
                        //N entrance
                        if(roomGrid[i, j].entranceDirs[0])
                        {
                            //S entrance, entrances opposite
                            if (roomGrid[i, j].entranceDirs[1])
                            {
                                rotation = 0f;
                                if (roomGrid[i, j].hallway) //instantiate straight hallway
                                {
                                    Instantiate(hallPrefabs[1], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                                else //instantiate straight room
                                {
                                    Instantiate(roomPrefabs[1], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                            }
                            //E entrance, entrances adjacent
                            else if (roomGrid[i, j].entranceDirs[2])
                            {
                                rotation = 0f;
                                if (roomGrid[i, j].hallway)
                                {
                                    Instantiate(hallPrefabs[2], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                                else
                                {
                                    Instantiate(roomPrefabs[2], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                            }
                            //W entrance, entrances adjacent
                            else
                            {
                                rotation = 270f;
                                if (roomGrid[i, j].hallway)
                                {
                                    Instantiate(hallPrefabs[2], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                                else
                                {
                                    Instantiate(roomPrefabs[2], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                            }
                        }

                        //E entrance
                        else if (roomGrid[i, j].entranceDirs[2])
                        {
                            //W entrance, entrances opposite
                            if (roomGrid[i, j].entranceDirs[3])
                            {
                                rotation = 90f;
                                if (roomGrid[i, j].hallway)
                                {
                                    Instantiate(hallPrefabs[1], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                                else
                                {
                                    Instantiate(roomPrefabs[1], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                            }
                            //S entrance, entrances adjacent
                            else if (roomGrid[i, j].entranceDirs[1])
                            {
                                rotation = 90f;
                                if (roomGrid[i, j].hallway)
                                {
                                    Instantiate(hallPrefabs[2], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                                else
                                {
                                    Instantiate(roomPrefabs[2], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                            }
                        }

                        //S entrance
                        else if (roomGrid[i, j].entranceDirs[1])
                        {
                            //W entrance, entrances adjacent
                            if (roomGrid[i, j].entranceDirs[3])
                            {
                                rotation = 180f;
                                if (roomGrid[i, j].hallway)
                                {
                                    Instantiate(hallPrefabs[2], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                                else
                                {
                                    Instantiate(roomPrefabs[2], currentPos, Quaternion.Euler(0f, rotation, 0f));
                                }
                            }
                        }
                    }

                    //room has 1 entrance, rotate based on which wall has entrance, default is N
                    else
                    {
                        //N entrance
                        if (roomGrid[i, j].entranceDirs[0])
                        {
                            rotation = 0f;
                        }
                        //S entrance
                        else if (roomGrid[i, j].entranceDirs[1])
                        {
                            rotation = 180f;
                        }
                        //E entrance
                        else if (roomGrid[i, j].entranceDirs[2])
                        {
                            rotation = 90f;
                        }
                        //W entrance
                        else
                        {
                            rotation = 270f;
                        }

                        //instantiate 1-entrance hallway
                        if (roomGrid[i, j].hallway)
                        {
                            Instantiate(hallPrefabs[0], currentPos, Quaternion.Euler(0f, rotation, 0f));
                        }
                        //instantiate 1-entrance room
                        else
                        {
                            Instantiate(roomPrefabs[0], currentPos,Quaternion.Euler(0f, rotation, 0f));
                        }
                    }
                }

                //update position information to next column
                currentPos.z += roomSize;
            }

            //update position to start next row back at z = 0
            currentPos.z = 0f;
            currentPos.x += roomSize;
        }
    }

    private bool ListContains(List<int[]> list, int[] search)
    {
        bool found = false;
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i][0] == search[0] && list[i][1] == search[1])
            {
                found = true;
                break;
            }
        }

        return found;
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
                    printStr += " ";
                    if (roomGrid[i, j].hallway)
                    {
                        printStr += "H";
                    }
                    printStr += roomGrid[i, j].numEntrances + "["; //print number of entrances if there is a room here
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
