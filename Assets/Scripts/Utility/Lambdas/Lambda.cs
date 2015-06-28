using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lambdas
{
    public static partial class Lambda
    {

        public static Func<Ta, Tr> Memoized<Ta, Tr>(Func<Ta, Tr> fun) {
            Dictionary<Ta, Tr> memoized = new Dictionary<Ta, Tr>();
            return (Ta arg) =>
            {
                Tr result;
                if (!memoized.TryGetValue(arg, out result))
                {
                    result = fun(arg);
                    memoized[arg] = result;
                }
                return result;
            };
        }

        public static Func<Ta, Tr> ObserveCalls<Ta, Tr>(Func<Ta, Tr> fun, Action<Tr> observer) {
            return arg =>
            {
                var result = fun(arg);
                observer(result);
                return result;
            };
        }
    }
}
