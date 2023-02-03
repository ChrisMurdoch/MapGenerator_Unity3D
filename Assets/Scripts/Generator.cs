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

    }

    /// <summary>
    /// Generates a grid of 
    /// </summary>
    private void GenerateGrid()
    {
        for(int i = 0; i < maxRows; i++)
        {
            for(int j = 0; j < maxColumns; j++)
            {
                // 0 represents empty space, 1 represents a room
                int num = Random.Range(0, 2);
            }
        }
    }

    /// <summary>
    /// simple debug function to print results of grid generation
    /// </summary>
    private void DebugPrintGrid()
    {
        string printString = generatedGrid[0,0].ToString();
        for(int i = 0; i < generatedGrid.GetLength(0); i++)
        {
            for(int j = 0; j < generatedGrid.GetLength(1); j++)
            {
                printString += " " + generatedGrid[i, j].ToString(); //spaces between values in each column
            }

            printString += "\n"; //new line for each row
        }

        Debug.Log(printString);
    }
}
