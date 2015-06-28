using UnityEngine;
using System.Collections;
using Lambdas;

namespace Lambdas
{
    public static partial class Lambda
    {

		public delegate Stream<Targs> Stream<Targs>(Targs args);

        public static Stream<Targs> NullStream<Targs>(Targs args)
        {
            return NullStream;
        }
    }
}
