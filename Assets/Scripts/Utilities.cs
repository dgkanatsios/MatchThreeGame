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

    /// <summary>
    /// Checks if a shape is next to another one
    /// either horizontally or vertically
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <returns></returns>
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
        //list that will contain all the matches we find
        List<List<GameObject>> matches = new List<List<GameObject>>();
       
        for (int row = 0; row < Constants.Rows; row++)
        {
            for (int column = 0; column < Constants.Columns; column++)
            {

                var matches1 = CheckHorizontal1(row, column, shapes);
                var matches2 = CheckHorizontal2(row, column, shapes);
                var matches3 = CheckHorizontal3(row, column, shapes);
                var matches4 = CheckVertical1(row, column, shapes);
                var matches5 = CheckVertical2(row, column, shapes);
                var matches6 = CheckVertical3(row, column, shapes);

                if (matches1 != null) matches.Add(matches1);
                if (matches2 != null) matches.Add(matches2);
                if (matches3 != null) matches.Add(matches3);
                if (matches4 != null) matches.Add(matches4);
                if (matches5 != null) matches.Add(matches5);
                if (matches6 != null) matches.Add(matches6);

                //if we have >= 3 matches, return a random one
                if (matches.Count >= 3)
                    return matches[UnityEngine.Random.Range(0, matches.Count - 1)];

                //if we are in the middle of the calculations/loops
                //and we have less than 3 matches, return a random one
                if(row >= Constants.Rows / 2 && matches.Count > 0 && matches.Count <=2)
                    return matches[UnityEngine.Random.Range(0, matches.Count - 1)];
            }
        }
        return null;
    }

    public static List<GameObject> CheckHorizontal1(int row, int column, ShapesArray shapes)
    {
        if (column <= Constants.Columns - 2)
        {
            if (shapes[row, column].GetComponent<Shape>().
                IsSameType(shapes[row, column + 1].GetComponent<Shape>()))
            {
                if (row >= 1 && column >= 1)
                    if (shapes[row, column].GetComponent<Shape>().
                    IsSameType(shapes[row - 1, column - 1].GetComponent<Shape>()))
                        return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row, column + 1],
                                    shapes[row - 1, column - 1]
                                };

                /* example *\
                 * * * * *
                 * * * * *
                 * * * * *
                 * & & * *
                 & * * * *
                \* example  */

                if (row <= Constants.Rows - 2 && column >= 1)
                    if (shapes[row, column].GetComponent<Shape>().
                    IsSameType(shapes[row + 1, column - 1].GetComponent<Shape>()))
                        return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row, column + 1],
                                    shapes[row + 1, column - 1]
                                };

                /* example *\
                 * * * * *
                 * * * * *
                 & * * * *
                 * & & * *
                 * * * * *
                \* example  */
            }
        }
        return null;
    }


    public static List<GameObject> CheckHorizontal2(int row, int column, ShapesArray shapes)
    {
        if (column <= Constants.Columns - 3)
        {
            if (shapes[row, column].GetComponent<Shape>().
                IsSameType(shapes[row, column + 1].GetComponent<Shape>()))
            {

                if (row >= 1 && column <= Constants.Columns - 3)
                    if (shapes[row, column].GetComponent<Shape>().
                    IsSameType(shapes[row - 1, column + 2].GetComponent<Shape>()))
                        return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row, column + 1],
                                    shapes[row - 1, column + 2]
                                };

                /* example *\
                 * * * * *
                 * * * * *
                 * * * * *
                 * & & * *
                 * * * & *
                \* example  */

                if (row <= Constants.Rows - 2 && column <= Constants.Columns - 3)
                    if (shapes[row, column].GetComponent<Shape>().
                    IsSameType(shapes[row + 1, column + 2].GetComponent<Shape>()))
                        return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row, column + 1],
                                    shapes[row + 1, column + 2]
                                };

                /* example *\
                 * * * * *
                 * * * * *
                 * * * & *
                 * & & * *
                 * * * * *
                \* example  */
            }
        }
        return null;
    }

    public static List<GameObject> CheckHorizontal3(int row, int column, ShapesArray shapes)
    {
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

            /* example *\
              * * * * *  
              * * * * *
              * * * * *
              * & & * &
              * * * * *
            \* example  */
        }
        if (column >= 2 && column <= Constants.Columns - 2)
        {
            if (shapes[row, column].GetComponent<Shape>().
               IsSameType(shapes[row, column + 1].GetComponent<Shape>()) &&
               shapes[row, column].GetComponent<Shape>().
               IsSameType(shapes[row, column - 2].GetComponent<Shape>()))
            {
                return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row, column + 1],
                                    shapes[row, column -2]
                                };
            }

            /* example *\
              * * * * * 
              * * * * *
              * * * * *
              * & * & &
              * * * * *
            \* example  */
        }
        return null;
    }

    public static List<GameObject> CheckVertical1(int row, int column, ShapesArray shapes)
    {
        if (row <= Constants.Rows - 2)
        {
            if (shapes[row, column].GetComponent<Shape>().
                IsSameType(shapes[row + 1, column].GetComponent<Shape>()))
            {
                if (column >= 1 && row >= 1)
                    if (shapes[row, column].GetComponent<Shape>().
                    IsSameType(shapes[row - 1, column - 1].GetComponent<Shape>()))
                        return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row + 1, column],
                                    shapes[row - 1, column -1]
                                };

                /* example *\
                  * * * * *
                  * * * * *
                  * & * * *
                  * & * * *
                  & * * * *
                \* example  */

                if (column <= Constants.Columns - 2 && row >= 1)
                    if (shapes[row, column].GetComponent<Shape>().
                    IsSameType(shapes[row - 1, column + 1].GetComponent<Shape>()))
                        return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row + 1, column],
                                    shapes[row - 1, column + 1]
                                };

                /* example *\
                  * * * * *
                  * * * * *
                  * & * * *
                  * & * * *
                  * * & * *
                \* example  */
            }
        }
        return null;
    }

    public static List<GameObject> CheckVertical2(int row, int column, ShapesArray shapes)
    {
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
                                    shapes[row + 1, column],
                                    shapes[row + 2, column -1]
                                };

                /* example *\
                  * * * * *
                  & * * * *
                  * & * * *
                  * & * * *
                  * * * * *
                \* example  */

                if (column <= Constants.Columns - 2)
                    if (shapes[row, column].GetComponent<Shape>().
                    IsSameType(shapes[row + 2, column + 1].GetComponent<Shape>()))
                        return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row+1, column],
                                    shapes[row + 2, column + 1]
                                };

                /* example *\
                  * * * * *
                  * * & * *
                  * & * * *
                  * & * * *
                  * * * * *
                \* example  */

            }
        }
        return null;
    }

    public static List<GameObject> CheckVertical3(int row, int column, ShapesArray shapes)
    {
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

        /* example *\
          * & * * *
          * * * * *
          * & * * *
          * & * * *
          * * * * *
        \* example  */

        if (row >= 2 && row <= Constants.Rows - 2)
        {
            if (shapes[row, column].GetComponent<Shape>().
               IsSameType(shapes[row + 1, column].GetComponent<Shape>()) &&
               shapes[row, column].GetComponent<Shape>().
               IsSameType(shapes[row - 2, column].GetComponent<Shape>()))
            {
                return new List<GameObject>()
                                {
                                    shapes[row, column],
                                    shapes[row + 1, column],
                                    shapes[row - 2, column]
                                };
            }
        }

        /* example *\
          * * * * *
          * & * * *
          * & * * *
          * * * * *
          * & * * *
        \* example  */
        return null;
    }


}

