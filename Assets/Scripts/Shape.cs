using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public int Column { get; set; }
    public int Row { get; set; }

    public string Type { get; set; }

    public Shape()
    { }

    public Shape(string type, int row, int column)
    {

        if (string.IsNullOrEmpty(type))
            throw new ArgumentException("type");

        Column = column;
        Row = row;
        Type = type;
    }

    public bool IsSameType(object otherShape)
    {
        if (otherShape == null || !(otherShape is Shape))
            throw new ArgumentException("otherShape");

        return string.Compare(this.Type, (otherShape as Shape).Type) == 0;
    }

    public void Assign(string type, int row, int column)
    {

        if (string.IsNullOrEmpty(type))
            throw new ArgumentException("type");

        Column = column;
        Row = row;
        Type = type;
    }

    public static void SwapColumnRow(Shape a, Shape b)
    {
        int temp = a.Row;
        a.Row = b.Row;
        b.Row = temp;

        temp = a.Column;
        a.Column = b.Column;
        b.Column = temp;
    }
}

