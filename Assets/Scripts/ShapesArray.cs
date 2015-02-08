using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class ShapesArray
{

    private GameObject[,] shapes = new GameObject[Constants.Rows, Constants.Columns];

    /// <summary>
    /// Indexer
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Swaps the position of two items, also keeping a backup
    /// </summary>
    /// <param name="g1"></param>
    /// <param name="g2"></param>
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

        Shape.SwapColumnRow(g1Shape, g2Shape);

    }

    /// <summary>
    /// Undoes the swap
    /// </summary>
    public void UndoSwap()
    {
        if (backupG1 == null || backupG2 == null)
            throw new Exception("Backup is null");

        Swap(backupG1, backupG2);
    }

    private GameObject backupG1;
    private GameObject backupG2;


    public IEnumerable<GameObject> GetMatches(IEnumerable<GameObject> gos)
    {
        List<GameObject> matches = new List<GameObject>();
        foreach (var go in gos)
        {
            matches.AddRange(GetMatches(go));
        }
        return matches.Distinct();
    }

    public IEnumerable<GameObject> GetMatches(GameObject go)
    {
        return GetMatchesHorizontally(go).Union(GetMatchesVertically(go)).Distinct();
    }

    private IEnumerable<GameObject> GetMatchesHorizontally(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<Shape>();
        //check left
        if (shape.Column != 0)
            for (int column = shape.Column - 1; column >= 0; column--)
            {
                if (shapes[shape.Row, column].GetComponent<Shape>().IsSameType(shape))
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
                if (shapes[shape.Row, column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[shape.Row, column]);
                }
                else
                    break;
            }

        //we want more than three matches
        if (matches.Count < 3)
            matches.Clear();

        return matches;
    }

    private IEnumerable<GameObject> GetMatchesVertically(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<Shape>();
        //check bottom
        if (shape.Row != 0)
            for (int row = shape.Row - 1; row >= 0; row--)
            {
                if (shapes[row, shape.Column] != null && shapes[row, shape.Column].GetComponent<Shape>().IsSameType(shape))
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
                if (shapes[row, shape.Column] != null && shapes[row, shape.Column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[row, shape.Column]);
                }
                else
                    break;
            }


        if (matches.Count < 3)
            matches.Clear();

        return matches;
    }

    public void Remove(GameObject item)
    {
        shapes[item.GetComponent<Shape>().Row, item.GetComponent<Shape>().Column] =
            null;
    }

    public IEnumerable<GameObject> Collapse(IEnumerable<int> columns)
    {
        List<GameObject> GOsMoved = new List<GameObject>();
        ///search in every column
        foreach (var column in columns)
        {
            //begin from bottom row
            for (int row = 0; row < Constants.Rows - 1; row++)
            {
                //if you find a null item
                if (shapes[row, column] == null)
                {
                    //start searching for the first non-null
                    for (int row2 = row + 1; row2 < Constants.Rows; row2++)
                    {
                        //if you find one, bring it down (i.e. replace it with the null you found)
                        if (shapes[row2, column] != null)
                        {
                            shapes[row, column] = shapes[row2, column];
                            shapes[row2, column] = null;

                            //assign new row and column (name does not change)
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

