using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class Player : Character
    {
        public string name
        {
            get { return "Player"; }
        }

        public Player()
            : base()
        {
            canMove = true;
        }
    }
}