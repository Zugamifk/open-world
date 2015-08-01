using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LinkedListx  {

	public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current) {
        if (current.Next == null)
            return current.List.First;
        return current.Next;
    }

    public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current) {
        if (current.Previous == null)
            return current.List.Last;
        return current.Previous;
    }
}
