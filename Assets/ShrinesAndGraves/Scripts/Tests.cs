using UnityEngine;
﻿using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI.Extensions;

namespace Shrines {
	public class Tests : MonoBehaviour {

		RectTransform tf;
		IntervalTree tree;

		// Use this for initialization
		void Start () {
			tf = GetComponent<RectTransform>();
			tree = new IntervalTree();
			float X0 = 0, X1 = 1000;
			float M = X1 - X0;
			for(int i = 0; i < 1000; i++) {
				var a = X0 + Random.value * M;
				var b = X0 + Random.value * M;
				var vec = new Vector2(Mathf.Min(a,b), Mathf.Max(a,b));
				tree.Insert(vec);
			}
			for(int i=0;i<5;i++) {
				var x = X0 + Random.value * M;
				Debug.Log(string.Format("Overlapping {0}: {1}", x, tree.Overlapping(x).Count()));
			}

            var y = 0;
            var cols = Colorx.FibonacciHues(Colorx.lightmaroon).GetEnumerator();
            foreach (var n in tree.Nodes)
            {
                var block = new GameObject(string.Format("Node on {2} spanning {0}-{1} at level {3}", n.min, n.max, n.center, n.depth), typeof(RectTransform));
                block.transform.SetParent(tf, false);
                var br = block.GetComponent<RectTransform>();
                SetRect(br);
                var h = 15 ;//+ n.sortedByMax.Count * 5;
                br.anchoredPosition = new Vector2(n.min, n.depth*h);
                br.sizeDelta = new Vector2(n.max - n.min, h);
                var img = block.AddComponent<Image>();
                cols.MoveNext();
                img.color = cols.Current;

                var linego = new GameObject("Line", typeof(RectTransform));
                linego.transform.SetParent(tf, false);
                var line = linego.AddComponent<UILineRenderer>();
                line.Points = new Vector2[] {
                    new Vector2(n.center, n.depth*h),
                    new Vector2(n.center, tf.rect.yMax)
                };
                var lr = line.GetComponent<RectTransform>();
                SetRect(lr);
                lr.sizeDelta = Vector2.one;
                Debug.Log(string.Format("{0} intervals on {1} at level {2}", n.sortedByMax.Count, n.center, n.depth));
            }

		}

        void SetRect(RectTransform rect)
        {
            rect.pivot = Vector2.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
        }

	}
}
