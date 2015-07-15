using UnityEngine;
using System.Collections;

public class Tuple<A, B> {
	public A First { get; set; }
	public B Second { get; set; }
	public Tuple(A first, B second) {
		First = first;
		Second = second;
	}
}

public class Tuple<A, B, C> {
	public A First { get; set; }
	public B Second { get; set; }
	public C Third { get; set; }
	public Tuple(A first, B second, C third) {
		First = first;
		Second = second;
		Third = third;
	}
}

public class Tuple<A, B, C, D> {
	public A First { get; set; }
	public B Second { get; set; }
	public C Third { get; set; }
	public D Fourth { get; set; }
	public Tuple(A first, B second, C third, D fourth) {
		First = first;
		Second = second;
		Third = third;
		Fourth = fourth;
	}
}
