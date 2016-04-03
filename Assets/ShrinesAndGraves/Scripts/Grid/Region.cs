using UnityEngine;
using System.Collections;
using Extensions;

namespace Shrines
{
    public class Region : ScriptableObject
    {
        public Environment environment;

        public Recti rect;

        public virtual void Fill(Grid g)
        {

        }
    }
}