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

    private IEnumerator CheckPotentialMatchesCoroutine;
    private IEnumerator AnimatePotentialMatchesCoroutine;


    // Use this for initialization
    void Start()
    {
        InitializeTypesOnPrefabShapes();

        InitializeCandyAndSpawnPositions();

        StartCheckForPotentialMatches();
    }

    /// <summary>
    /// Initialize shapes
    /// </summary>
    private void InitializeTypesOnPrefabShapes()
    {
        //just assign the name of the prefab
        foreach (var item in CandyPrefabs)
        {
            item.GetComponent<Shape>().Type = item.name;
        }
    }

    public void InitializeCandyAndSpawnPositions()
    {
        if (shapes != null)
            DestroyAllCandy();

        shapes = new ShapesArray();
        SpawnPositions = new Vector2[Constants.Columns];

        for (int row = 0; row < Constants.Rows; row++)
        {
            for (int column = 0; column < Constants.Columns; column++)
            {

                var newCandy = GetRandomCandy();

                //check if two previous horizontal are of the same type
                while (column >= 2 && shapes[row, column - 1].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>())
                    && shapes[row, column - 2].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>()))
                {
                    newCandy = GetRandomCandy();
                }

                //check if two previous vertical are of the same type
                while (row >= 2 && shapes[row - 1, column].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>())
                    && shapes[row - 2, column].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>()))
                {
                    newCandy = GetRandomCandy();
                }

                GameObject go = Instantiate(newCandy,
                    BottomRight + new Vector2(column * CandySize.x, row * CandySize.y), Quaternion.identity)
                    as GameObject;


                go.GetComponent<Shape>().Assign(newCandy.GetComponent<Shape>().Type, row, column);

                shapes[row, column] = go;

            }
        }

        //create the spawn positions for the new shapes (will pop from the 'ceiling')
        for (int column = 0; column < Constants.Columns; column++)
        {
            SpawnPositions[column] = BottomRight
                + new Vector2(column * CandySize.x, Constants.Rows * CandySize.y);
        }
    }


    /// <summary>
    /// Destroy all candy gameobjects
    /// </summary>
    private void DestroyAllCandy()
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

            //user has clicked or touched
            if (Input.GetMouseButtonDown(0))
            {
                //get the hit position
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null) //we have a hit!!!
                {
                    hitGo = hit.collider.gameObject;
                    state = State.SelectionStarted;
                }
                //user clicked/touched the screen, no need to show him hints for a shortwhile
                StopCheckForPotentialMatches();
            }
        }
        else if (state == State.SelectionStarted)
        {
            //user dragged
            if (Input.GetMouseButton(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                //we have a hit
                if (hit.collider != null && hitGo != hit.collider.gameObject)
                {
                    state = State.Animating;
                    StartCoroutine("FindMatchesAndCollapse", hit);
                }
            }
        }
    }




    private IEnumerator FindMatchesAndCollapse(object hit)
    {

        var hitGo2 = ((RaycastHit2D)hit).collider.gameObject;
        shapes.Swap(hitGo, hitGo2);

        //move the swapped ones
        hitGo.transform.positionTo(Constants.AnimationDuration, hitGo2.transform.position);
        hitGo2.transform.positionTo(Constants.AnimationDuration, hitGo.transform.position);
        yield return new WaitForSeconds(Constants.AnimationDuration);

        var sameShapes = shapes.GetMatches(hitGo)
            .Union(shapes.GetMatches(hitGo2)).Distinct();

        //if user's swap didn't create at least a 3-match, undo their swap
        if (sameShapes.Count() < 3)
        {
            hitGo.transform.positionTo(Constants.AnimationDuration, hitGo2.transform.position);
            hitGo2.transform.positionTo(Constants.AnimationDuration, hitGo.transform.position);
            yield return new WaitForSeconds(Constants.AnimationDuration);

            shapes.UndoSwap();
        }

        while (sameShapes.Count() >= 3)
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

            MoveAndAnimate(newCandies);
            MoveAndAnimate(movedGOs);

            //will wait for both of the above animations
            yield return new WaitForSeconds(Constants.AnimationDuration);

            //search if there are matches with the new/collapsed items
            sameShapes = shapes.GetMatches(movedGOs).Union(shapes.GetMatches(newCandies));

        }

        state = State.None;
        StartCheckForPotentialMatches();
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
                var go = GetRandomCandy();
                GameObject newCandy = Instantiate(go, SpawnPositions[column], Quaternion.identity)
                    as GameObject;

                newCandy.GetComponent<Shape>().Assign(go.GetComponent<Shape>().Type, item.Row, item.Column);

                shapes[item.Row, item.Column] = newCandy;
                newCandies.Add(newCandy);
            }
        }
        return newCandies.ToArray();
    }

    /// <summary>
    /// Animates gameobjects to their new position
    /// </summary>
    /// <param name="movedGOs"></param>
    private void MoveAndAnimate(IEnumerable<GameObject> movedGOs)
    {
        foreach (var item in movedGOs)
        {
            item.transform.positionTo(Constants.AnimationDuration, BottomRight +
                new Vector2(item.GetComponent<Shape>().Column * CandySize.x, item.GetComponent<Shape>().Row * CandySize.y));
        }
    }

    /// <summary>
    /// Destroys the candy and instantiates a new explosion gameobject
    /// </summary>
    /// <param name="item"></param>
    private void DestroyCandy(GameObject item)
    {
        GameObject explosion = GetRandomExplosion();
        var newExplosion = Instantiate(explosion, item.transform.position, Quaternion.identity) as GameObject;
        Destroy(newExplosion, Constants.ExplosionDuration);
        Destroy(item);
    }

    /// <summary>
    /// Get a random candy
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomCandy()
    {
        return CandyPrefabs[Random.Range(0, CandyPrefabs.Length)];
    }

    /// <summary>
    /// Get a random explosion
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomExplosion()
    {
        return ExplosionPrefabs[Random.Range(0, ExplosionPrefabs.Length)];
    }


    private void StartCheckForPotentialMatches()
    {
        StopCheckForPotentialMatches();
        //get a reference to stop it later
        CheckPotentialMatchesCoroutine = CheckPotentialMatches();
        StartCoroutine(CheckPotentialMatchesCoroutine);
    }

    private void StopCheckForPotentialMatches()
    {
        if (AnimatePotentialMatchesCoroutine != null)
            StopCoroutine(AnimatePotentialMatchesCoroutine);
        if (CheckPotentialMatchesCoroutine != null)
            StopCoroutine(CheckPotentialMatchesCoroutine);
        ResetOpacityOnPotentialMatches();
    }


    private void ResetOpacityOnPotentialMatches()
    {
        if (potentialMatches != null)
            foreach (var item in potentialMatches)
            {
                if (item == null) break;

                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = 1.0f;
                item.GetComponent<SpriteRenderer>().color = c;
            }
    }

    /// <summary>
    /// Finds potential matches
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckPotentialMatches()
    {
        yield return new WaitForSeconds(Constants.WaitBeforePotentialMatchesCheck);
        potentialMatches = shapes.GetPotentialMatches();
        while (true)
        {
            if (potentialMatches != null)
            {
                AnimatePotentialMatchesCoroutine = Utilities.AnimatePotentialMatches(potentialMatches);
                StartCoroutine(AnimatePotentialMatchesCoroutine);
                yield return new WaitForSeconds(Constants.WaitBeforePotentialMatchesCheck);
            }
        }
    }

    IEnumerable<GameObject> potentialMatches;

   

    enum State
    {
        None,
        SelectionStarted,
        Animating
    }
}
