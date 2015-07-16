using UnityEngine;
using System.Collections;
using System;
using Lambdas;

namespace Extensions {

	public static partial class Math {

		public static Pure1<float> Const0 = t => 0;
		public static Pure1<float> Const1 = t => 1;
		public static Pure2<float> ConstN = n => t => n;

	}
}
