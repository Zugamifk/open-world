using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    public class DialogTree : ScriptableObject
    {
        public static HashSet<string> flags = new HashSet<string>();

        [System.Serializable]
        public class Line
        {
            public string character;
            [TextArea]
            public string line;
        }

        [System.Serializable]
        public class Node
        {
            public Line[] lines;
            public BranchType branchType; // how the dialog branches here
            public string[] keys;
            public int[] branches;
        }

        public enum BranchType
        {
            DialogChoice, // open a dialog menu for selecting a reply
            FlagSet // check if a static flag has been set
        }

        [SerializeField]
        int root;
        [SerializeField]
        Node[] nodes;

        public Node Root
        {
            get
            {
                return nodes[root];
            }
        }

        public Node this[int i]
        {
            get
            {
                return nodes[i];
            }
        }
    }
}