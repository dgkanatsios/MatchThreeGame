using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class DebugUtilities
{
    public static void DebugRotate(GameObject go)
    {
        go.transform.Rotate(0, 0, 80f);
    }

    public static void DebugAlpha(GameObject go)
    {
        Color c = go.GetComponent<SpriteRenderer>().color;
        c.a = 0.6f;
        go.GetComponent<SpriteRenderer>().color = c;
    }

    public static void DebugPositions(GameObject hitGo, GameObject hitGo2)
    {
        string lala =
                        hitGo.GetComponent<Shape>().Row + "-"
                        + hitGo.GetComponent<Shape>().Column + "-"
                         + hitGo2.GetComponent<Shape>().Row + "-"
                         + hitGo2.GetComponent<Shape>().Column;
        Debug.Log(lala);

    }

    public static void ShowArray(ShapesArray shapes)
    {
        for (int row = Constants.Rows - 1; row >= 0; row--)
        {
            string x = string.Empty;
            for (int column = 0; column < Constants.Columns; column++)
            {
                if (shapes[row, column] == null)
                    x += "NULL |";
                else
                    x += row + "-" + column + " | ";
            }
            Debug.Log(x);
        }
    }
}

