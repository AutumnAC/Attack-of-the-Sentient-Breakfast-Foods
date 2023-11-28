using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;


// Class purpose: Create a flow field for the pancakes to follow. Currently just a stub.

public class FlowField : Singleton<FlowField>
{
    private const int columns = 160;
    private const int rows = 100;

    // The 2D array holding the flow field vectors
    private Vector3[,] flowField = new Vector3[columns, rows];


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if ((i < 30 && i > 10)
                    || (i > 30 && i < 60 && j > columns / 2))
                {
                    flowField[i, j] = new Vector3(-1, 0);
                }

                else if ((i > 30 && i < 60 && j < columns / 2)
                    || i > 60)
                {
                    flowField[i, j] = new Vector3(1, 0);
                }

                else if (i < 10)
                {
                    flowField[i, j] = new Vector3(0, -1);
                }

                else if (i > 90)
                {
                    flowField[i, j] = new Vector3(0, 1);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetFlowFieldPosition(Vector3 position)
    {
        int column = (int)Mathf.Round(position.x / columns);
        int row = (int)Mathf.Round(position.y / rows);
        return flowField[column, row];
    }
}
