using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class Utilities
{
    public static void SwapShape(Shape a, Shape b)
    {
        int temp = a.Row;
        a.Row = b.Row;
        b.Row = temp;

        temp = a.Column;
        a.Column = b.Column;
        b.Column = temp;
    }

    public static void DebugRotate(GameObject go)
    {
        //go.transform.Rotate(0, 0, 80f);
    }

    public static void DebugAlpha(GameObject go)
    {
        //Color c = go.GetComponent<SpriteRenderer>().color;
        //c.a = 0.6f;
        //go.GetComponent<SpriteRenderer>().color = c;
    }

    public static void DebugPositions(GameObject hitGo, GameObject hitGo2)
    {
        //string lala =
        //                hitGo.GetComponent<Shape>().Row + "-"
        //                + hitGo.GetComponent<Shape>().Column + "-"
        //                 + hitGo2.GetComponent<Shape>().Row + "-"
        //                 + hitGo2.GetComponent<Shape>().Column;
        //Debug.Log(lala);

    }
}

