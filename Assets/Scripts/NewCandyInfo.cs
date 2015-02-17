using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class NewCandyInfo
{
    private List<GameObject> newCandy { get;  set; }
        public int MaxDistance { get; set; }

        public IEnumerable<GameObject> DistinctNewCandy
        {
            get
            {
                return newCandy.Distinct();
            }
        }

        public void AddGameObject(GameObject go)
        {
            newCandy.Add(go);
        }

        public NewCandyInfo()
        {
            newCandy = new List<GameObject>();
        }
}
