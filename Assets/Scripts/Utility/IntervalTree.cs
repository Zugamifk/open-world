using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class IntervalTree {

    public class Interval {
        public Vector2 points;
        public bool marked; // for range queries
        public Interval(Vector2 points) {
            this.points = points;
        }
    }

	public class Node {
        public float center;
        public int depth = 0;
        public Node left;
        public Node right;
        public List<Interval> sortedByMin = new List<Interval>();
        public List<Interval> sortedByMax = new List<Interval>();

		struct MinSorter : IComparer<Interval> {
            public int Compare(Interval a, Interval b)
            {
                var diff = b.points.x - a.points.x;
                return diff > 0 ? 1 : diff < 0 ? -1 : 0;
            }
        }
        static MinSorter s_MinSorter;

        struct MaxSorter : IComparer<Interval>
        {
            public int Compare(Interval a, Interval b)
            {
                var diff = a.points.y - b.points.y;
                return diff > 0 ? 1 : diff < 0 ? -1 : 0;
            }
        }
        static MaxSorter s_MaxSorter;

		public float min {
			get {
                return sortedByMin[0].points.x;
			}
		}

		public float max {
			get {
                return sortedByMax[0].points.y;
			}
		}

        public Node(Interval value){
            center = value.points.x + (value.points.y - value.points.x) / 2;
            sortedByMax.Add(value);
            sortedByMin.Add(value);
        }

		public void Add(Interval value) {
			if (!Contains(value)) {
                if (value.points.y < center)
                {
					if(left == null) {
						left = new Node(value);
                        left.depth = depth + 1;
					} else {    
						left.Add(value);
					}
				} else {
					if(right == null) {
						right = new Node(value);
                        right.depth = depth + 1;
					} else {
						right.Add(value);
					}
				}
			} else {
	            sortedByMax.Add(value);
				sortedByMax.Sort(s_MaxSorter);
	            sortedByMin.Add(value);
				sortedByMax.Sort(s_MinSorter);
			}
		}

		public bool Contains(Interval value) {
			return value.points.x <= center && value.points.y >= center;
		}

		public IEnumerable<Interval> Containing(float p) {
			if(p > center) {
				foreach(var v in sortedByMax) {
					if(v.points.y < p) {
						yield break;
					} else {
						yield return v;
					}
				}
			} else {
				foreach(var v in sortedByMin) {
					if(v.points.x > p) {
						yield break;
					} else {
						yield return v;
					}
				}
			}
		}

        //public IEnumerable<Interval> Containing(Vector2 v)
        //{

        //}
    }

    Node root;

	public IntervalTree() {

    }

	public void Insert(Vector2 value) {
        var interval = new Interval(value);
		if (root == null) {
            root = new Node(interval);
        } else {
            root.Add(interval);
		}
	}

    public IEnumerable<Node> Nodes
    {
        get
        {
            var node = root;
            Stack<Node> stack = new Stack<Node>();
            while (node != null || stack.Count != 0) {
                if (node != null)
                {
                    stack.Push(node);
                    node = node.left;
                }
                else
                {
                    node = stack.Pop();
                    yield return node;
                    node = node.right;
                }
            }
        }
    }

	public IEnumerable<Vector2> Overlapping(float p) {
		Node node = root;
		while (node !=null) {
			foreach(var v in node.Containing(p)) {
				yield return v.points;
			}
			if (p > node.center) {
				node = node.right;
			} else if (p < node.center) {
				node = node.left;
			}
		}
	}
}
