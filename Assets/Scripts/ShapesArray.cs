using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class ShapesArray
{

    private GameObject[,] shapes = new GameObject[Constants.Rows, Constants.Columns];

    public GameObject this[int row, int column]
    {
        get
        {
            return shapes[row, column];
        }
        set
        {
            shapes[row, column] = value;
        }
    }

    public void Swap(GameObject g1, GameObject g2)
    {

        backupG1 = g1;
        backupG2 = g2;

        var g1Shape = g1.GetComponent<Shape>();
        var g2Shape = g2.GetComponent<Shape>();

        int g1Row = g1Shape.Row;
        int g1Column = g1Shape.Column;
        int g2Row = g2Shape.Row;
        int g2Column = g2Shape.Column;

        var temp = shapes[g1Row, g1Column];
        shapes[g1Row, g1Column] = shapes[g2Row, g2Column];
        shapes[g2Row, g2Column] = temp;

        Utilities.SwapShape(g1Shape, g2Shape);

    }

    public void UndoSwap()
    {
        if (backupG1 == null || backupG2 == null)
            throw new Exception("Backup is null");

        Swap(backupG1, backupG2);
    }

    private GameObject backupG1;
    private GameObject backupG2;


    public GameObject[] GetMatches(GameObject go)
    {
        return GetMatchesHorizontally(go).Union(GetMatchesVertically(go)).ToArray();
    }

    private GameObject[] GetMatchesHorizontally(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<Shape>();
        //check left
        if (shape.Column != 0)
            for (int column = shape.Column - 1; column >= 0; column--)
            {
                if (shapes[shape.Row, column].name == go.name)
                {
                    matches.Add(shapes[shape.Row, column]);
                }
                else
                    break;
            }

        //check right
        if (shape.Column != Constants.Columns - 1)
            for (int column = shape.Column + 1; column < Constants.Columns; column++)
            {
                if (shapes[shape.Row, column].name == go.name)
                {
                    matches.Add(shapes[shape.Row, column]);
                }
                else
                    break;
            }

        if (matches.Count < 3)
            matches.Clear();

        return matches.ToArray();
    }

    private GameObject[] GetMatchesVertically(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<Shape>();
        //check bottom
        if (shape.Row != 0)
            for (int row = shape.Row - 1; row >= 0; row--)
            {
                if (shapes[row, shape.Column] != null && shapes[row, shape.Column].name == go.name)
                {
                    matches.Add(shapes[row, shape.Column]);
                }
                else
                    break;
            }

        //check top
        if (shape.Row != Constants.Rows - 1)
            for (int row = shape.Row + 1; row < Constants.Rows; row++)
            {
                if (shapes[row, shape.Column] != null && shapes[row, shape.Column].name == go.name)
                {
                    matches.Add(shapes[row, shape.Column]);
                }
                else
                    break;
            }


        if (matches.Count < 3)
            matches.Clear();

        return matches.ToArray();
    }

    public void Remove(GameObject item)
    {
        shapes[item.GetComponent<Shape>().Row, item.GetComponent<Shape>().Column] =
            null;
    }

    public IEnumerable<GameObject> Collapse(IEnumerable<int> columns)
    {
        List<GameObject> GOsMoved = new List<GameObject>();

        foreach (var column in columns)
        {
            for (int row = 0; row < Constants.Rows - 1; row++)
            {
                if (shapes[row, column] == null)
                {
                    for (int row2 = row + 1; row2 < Constants.Rows; row2++)
                    {
                        if (shapes[row2, column] != null)
                        {
                            shapes[row, column] = shapes[row2, column];
                            shapes[row2, column] = null;

                            shapes[row, column].GetComponent<Shape>().Row = row;
                            shapes[row, column].GetComponent<Shape>().Column = column;

                            GOsMoved.Add(shapes[row, column]);
                            break;
                        }
                    }
                }
            }
        }

        return GOsMoved.Distinct();
    }

    public IEnumerable<Shape> GetEmptyItemsOnColumn(int column)
    {
        List<Shape> emptyItems = new List<Shape>();
        for (int row = 0; row < Constants.Rows; row++)
        {
            if (shapes[row, column] == null)
                emptyItems.Add(new Shape() { Row = row, Column = column });
        }
        return emptyItems;
    }
}

