using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class Utilities
{
    /// <summary>
    /// Helper method to animate potential matches
    /// </summary>
    /// <param name="potentialMatches"></param>
    /// <returns></returns>
    public static IEnumerator AnimatePotentialMatches(IEnumerable<GameObject> potentialMatches)
    {
        for (float i = 1f; i >= 0.3f; i -= 0.1f)
        {
            foreach (var item in potentialMatches)
            {
                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = i;
                item.GetComponent<SpriteRenderer>().color = c;
            }
            yield return new WaitForSeconds(Constants.OpacityAnimationFrameDelay);
        }
        for (float i = 0.3f; i <= 1f; i += 0.1f)
        {
            foreach (var item in potentialMatches)
            {
                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = i;
                item.GetComponent<SpriteRenderer>().color = c;
            }
            yield return new WaitForSeconds(Constants.OpacityAnimationFrameDelay);
        }
    }

    public static bool AreVerticalOrHorizontalNeighbors(Shape s1, Shape s2)
    {
        return (s1.Column == s2.Column ||
                        s1.Row == s2.Row)
                        && Mathf.Abs(s1.Column - s2.Column) <= 1
                        && Mathf.Abs(s1.Row - s2.Row) <= 1;
    }

    /// <summary>
    /// Will check for potential matches vertically and horizontally
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<GameObject> GetPotentialMatches(ShapesArray shapes)
    {
        List<GameObject> matches = null;
        for (int row = 0; row < Constants.Rows; row++)
        {
            for (int column = 0; column < Constants.Columns; column++)
            {
                //check horizontal
                if (column <= Constants.Columns - 3)
                {
                    if (shapes[row, column].GetComponent<Shape>().
                        IsSameType(shapes[row, column + 1].GetComponent<Shape>()))
                    {
                        if (row >= 1)
                            if (shapes[row, column].GetComponent<Shape>().
                            IsSameType(shapes[row - 1, column + 2].GetComponent<Shape>()))
                                return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row, column + 1],
                                    shapes[row - 1, column + 2]
                                };

                        if (row <= Constants.Rows - 2)
                            if (shapes[row, column].GetComponent<Shape>().
                            IsSameType(shapes[row + 1, column + 2].GetComponent<Shape>()))
                                return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row, column + 1],
                                    shapes[row + 1, column + 2]
                                };
                    }
                }
                if (column <= Constants.Columns - 4)
                {
                    if (shapes[row, column].GetComponent<Shape>().
                       IsSameType(shapes[row, column + 1].GetComponent<Shape>()) &&
                       shapes[row, column].GetComponent<Shape>().
                       IsSameType(shapes[row, column + 3].GetComponent<Shape>()))
                    {
                        return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row, column + 1],
                                    shapes[row, column + 3]
                                };
                    }
                }

                //check vertical
                if (row <= Constants.Rows - 3)
                {
                    if (shapes[row, column].GetComponent<Shape>().
                        IsSameType(shapes[row + 1, column].GetComponent<Shape>()))
                    {
                        if (column >= 1)
                            if (shapes[row, column].GetComponent<Shape>().
                            IsSameType(shapes[row + 2, column - 1].GetComponent<Shape>()))
                                return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row+1, column],
                                    shapes[row + 2, column -1]
                                };

                        if (column <= Constants.Columns - 2)
                            if (shapes[row, column].GetComponent<Shape>().
                            IsSameType(shapes[row + 2, column + 1].GetComponent<Shape>()))
                                return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row+1, column],
                                    shapes[row + 2, column + 1]
                                };
                    }
                }
                if (row <= Constants.Rows - 4)
                {
                    if (shapes[row, column].GetComponent<Shape>().
                       IsSameType(shapes[row + 1, column].GetComponent<Shape>()) &&
                       shapes[row, column].GetComponent<Shape>().
                       IsSameType(shapes[row + 3, column].GetComponent<Shape>()))
                    {
                        return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row + 1, column],
                                    shapes[row + 3, column]
                                };
                    }
                }
            }
        }
        return matches;
    }

   
}

