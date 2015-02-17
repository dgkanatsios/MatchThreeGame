using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


    public class CollapseInfo
    {
        private List<GameObject> collapsed { get;  set; }
        public int MaxDistance { get; set; }

        public IEnumerable<GameObject> DistinctCollapsed
        {
            get
            {
                return collapsed.Distinct();
            }
        }

        public void AddGameObject(GameObject go)
        {
            collapsed.Add(go);
        }

        public CollapseInfo()
        {
            collapsed = new List<GameObject>();
        }
    }

