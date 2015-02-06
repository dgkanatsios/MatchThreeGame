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

    private State state = State.None;
    private GameObject hitGo = null;
    public Vector2[] SpawnPositions;
    public GameObject[] CandyPrefabs;
    public GameObject[] ExplosionPrefabs;

    // Use this for initialization
    void Start()
    {
        InitializeCandyAndSpawnPositions();
    }

    public void InitializeCandyAndSpawnPositions()
    {
        if (shapes != null)
            RemoveAllCandy();

        shapes = new ShapesArray();
        SpawnPositions = new Vector2[Constants.Columns];

        for (int row = 0; row < Constants.Rows; row++)
        {
            for (int column = 0; column < Constants.Columns; column++)
            {

                var newCandy = GetNewCandy();

                //check if two previous horizontal are of the same type
                while (column >= 2 && shapes[row, column - 1].name == newCandy.name && shapes[row, column - 2].name == newCandy.name)
                {
                    newCandy = GetNewCandy();
                }

                //check if two previous vertical are of the same type
                while (row >= 2 && shapes[row - 1, column].name == newCandy.name && shapes[row - 2, column].name == newCandy.name)
                {
                    newCandy = GetNewCandy();
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

        for (int column = 0; column < Constants.Columns; column++)
        {
            SpawnPositions[column] = BottomRight 
                + new Vector2(column * CandySize.x, Constants.Rows * CandySize.y);
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
                    state = State.Animating;
                    StartCoroutine("FindMatchedAndCollapse", hit);
                }
            }
        }
    }

    private IEnumerator FindMatchedAndCollapse(object hit)
    {

        var hitGo2 = ((RaycastHit2D)hit).collider.gameObject;
        shapes.Swap(hitGo, hitGo2);

        //move the swapped ones
        hitGo.transform.positionTo(Constants.AnimationDuration, hitGo2.transform.position);
        hitGo2.transform.positionTo(Constants.AnimationDuration, hitGo.transform.position);
        yield return new WaitForSeconds(Constants.AnimationDuration);

        var sameShapes = shapes.GetMatches(hitGo)
            .Union(shapes.GetMatches(hitGo2)).Distinct();

        if (sameShapes.Count() >= 3)
        {
            var columns = sameShapes.Select(x2 => x2.GetComponent<Shape>().Column).Distinct();
            foreach (var item in sameShapes)
            {
                shapes.Remove(item);
                DestroyCandy(item);
            }

            //the order the 2 methods below get called is of most importance!!!
            //collapse the ones gone
            var movedGOs = shapes.Collapse(columns);
            //create new ones
            var newCandies = CreateNewCandyInSpecificColumns(columns);

            Reposition(newCandies);
            Reposition(movedGOs);
            yield return new WaitForSeconds(Constants.AnimationDuration);
            DebugUtilities.ShowArray(shapes);
        }
        else
        {
            hitGo.transform.positionTo(Constants.AnimationDuration, hitGo2.transform.position);
            hitGo2.transform.positionTo(Constants.AnimationDuration, hitGo.transform.position);
            yield return new WaitForSeconds(Constants.AnimationDuration);

            shapes.UndoSwap();

        }
        state = State.None;
    }

   

    private GameObject[] CreateNewCandyInSpecificColumns(IEnumerable<int> columnsWithMissingCandy)
    {
        List<GameObject> newCandies = new List<GameObject>();
        //find how many null values the column has
        foreach (int column in columnsWithMissingCandy)
        {
            var emptyItems = shapes.GetEmptyItemsOnColumn(column);
            foreach (var item in emptyItems)
            {
                var go = GetNewCandy();
                GameObject newCandy = Instantiate(go, SpawnPositions[column], Quaternion.identity)
                    as GameObject;

                newCandy.name = go.name;
                newCandy.GetComponent<Shape>().Column = item.Column;
                newCandy.GetComponent<Shape>().Row = item.Row;
                shapes[item.Row, item.Column] = newCandy;
                newCandies.Add(newCandy);
            }
        }
        return newCandies.ToArray();
    }

    private void Reposition(IEnumerable<GameObject> movedGOs)
    {
        foreach (var item in movedGOs)
        {
            item.transform.positionTo(Constants.AnimationDuration, BottomRight +
                new Vector2(item.GetComponent<Shape>().Column * CandySize.x, item.GetComponent<Shape>().Row * CandySize.y));
        }
    }

    private  void DestroyCandy(GameObject item)
    {
        GameObject explosion = GetNewExplosion();
        var newExplosion = Instantiate(explosion, item.transform.position, Quaternion.identity) as GameObject;
        Destroy(newExplosion, Constants.ExplosionDuration);
        Destroy(item);
    }

    private GameObject GetNewCandy()
    {
        return CandyPrefabs[Random.Range(0, CandyPrefabs.Length)];
    }

    private GameObject GetNewExplosion()
    {
        return ExplosionPrefabs[Random.Range(0, ExplosionPrefabs.Length)];
    }

    enum State
    {
        None,
        SelectionStarted,
        Animating
    }
}
