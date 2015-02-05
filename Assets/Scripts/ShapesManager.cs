using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ShapesManager : MonoBehaviour
{


    //candy graphics taken from http://opengameart.org/content/candy-pack-1

    public ShapesArray shapes;

    public readonly Vector2 BottomRight = new Vector2(-2.37f, -4.27f);
    public readonly Vector2 CandySize = new Vector2(0.7f, 0.7f);



    public GameObject[] Prefabs;

    // Use this for initialization
    void Start()
    {
        InitializeCandy();
    }

    public void InitializeCandy()
    {
        if (shapes != null)
            RemoveAllCandy();

        shapes = new ShapesArray();
        for (int row = 0; row < Constants.Rows; row++)
        {
            for (int column = 0; column < Constants.Columns; column++)
            {

                var newCandy = Prefabs[Random.Range(0, Prefabs.Length)];

                //check if two previous horizontal are of the same type
                while (column >= 2 && shapes[row, column - 1].name == newCandy.name && shapes[row, column - 2].name == newCandy.name)
                {
                    newCandy = Prefabs[Random.Range(0, Prefabs.Length)];
                }

                //check if two previous vertical are of the same type
                while (row >= 2 && shapes[row - 1, column].name == newCandy.name && shapes[row - 2, column].name == newCandy.name)
                {
                    newCandy = Prefabs[Random.Range(0, Prefabs.Length)];
                }

                GameObject go = Instantiate(newCandy,
                    BottomRight + new Vector2(column * CandySize.x, row * CandySize.y), Quaternion.identity)
                    as GameObject;
                go.name = newCandy.name;
                go.GetComponent<Shape>().Row = row;
                go.GetComponent<Shape>().Column = column;
                shapes[row, column] = go;
            }
        }
    }

    private void RemoveAllCandy()
    {
        for (int row = 0; row < Constants.Rows; row++)
        {
            for (int column = 0; column < Constants.Columns; column++)
            {
                Destroy(shapes[row, column]);
            }
        }
    }

    private State state = State.None;
    private GameObject hitGo = null;
    // Update is called once per frame
    void Update()
    {
        if (state == State.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    hitGo = hit.collider.gameObject;
                    state = State.SelectionStarted;
                    Utilities.DebugAlpha(hitGo);
                }
            }
        }
        else if (state == State.SelectionStarted)
        {
            if (Input.GetMouseButton(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hitGo != hit.collider.gameObject)
                {
                    var hitGo2 = hit.collider.gameObject;
                    Utilities.DebugAlpha(hitGo2);
                    Utilities.DebugPositions(hitGo, hitGo2);
                    shapes.Swap(hitGo, hitGo2);
                    Utilities.DebugPositions(hitGo, hitGo2);

                    hitGo.transform.positionTo(1, hitGo2.transform.position);
                    hitGo2.transform.positionTo(1, hitGo.transform.position).setOnCompleteHandler(x1 =>
                        {
                            var sameShapes = shapes.GetMatches(hitGo)
                                .Union(shapes.GetMatches(hitGo2)).Distinct();

                            if (sameShapes.Count() >= 3)
                            {
                                var columns = sameShapes.Select(x2 => x2.GetComponent<Shape>().Column).Distinct();
                                foreach (var item in sameShapes)
                                {
                                    shapes.Remove(item);
                                    Destroy(item);
                                }

                                var movedGOs = shapes.Collapse(columns);
                                Reposition(movedGOs);
                            }
                            else
                            {
                                hitGo.transform.positionTo(1, hitGo2.transform.position);
                                hitGo2.transform.positionTo(1, hitGo.transform.position).setOnCompleteHandler(x3 =>
                                    {
                                        shapes.UndoSwap();
                                    });
                            }
                            state = State.None;

                        });
                    state = State.Animating;

                }
            }
        }
    }

    

    private void Reposition(IEnumerable<GameObject> movedGOs)
    {
        foreach (var item in movedGOs)
        {
            item.transform.positionTo(1f, BottomRight +
                new Vector2(item.GetComponent<Shape>().Column * CandySize.x, item.GetComponent<Shape>().Row * CandySize.y));
        }
    }

    enum State
    {
        None,
        SelectionStarted,
        Animating
    }
}
