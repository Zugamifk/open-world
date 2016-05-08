using System;

public static class ArrayExtensions {

	//Gets a random element from an array
	public static T Random<T>(this T[] array) {
        if (array == null || array.Length == 0)
        {
            return default(T);
        }
        else
        {
            return array[Randomx.Index<T>(array)];
        }
	}

	//Returns a new array that is a copy of the given array with the element at the specified index removed
	public static T[] RemoveAt<T>(T[] array, int index) {
		if (index < 0 || index >= array.Length) {
			throw new System.ArgumentOutOfRangeException("index", index, "Index must be in range [0,array.Length)");
		} else {
			var newArray = new T[array.Length - 1];
			for (int i = 0; i < array.Length; i++) {
				if (i == index) {
					continue;
				} else if (i < index) {
					newArray[i] = array[i];
				} else {
					newArray[i - 1] = array[i];
				}
			}
			return newArray;
		}
	}

	//Returns a new array that is a copy of the given array with a new element inserted at the specified index
	public static T[] InsertAt<T>(T[] array, T element, int index){
		if (index < 0 || index > array.Length) {
			throw new System.ArgumentOutOfRangeException("index", index, "Index must be in range [0,array.Length]");
		}
		var newArray = new T[array.Length + 1];
		for (int i = 0; i < newArray.Length; i++) {
			if (i == index) {
				newArray[i] = element;
			} else if (i < index) {
				newArray[i] = array[i];
			} else {
				newArray[i] = array[i-1];
			}
		}
		return newArray;
	}

	//Moves an element in an array from one index to another, preserving order of other elements.
	public static void Move<T>(T[] array, int fromIndex, int toIndex){
		if(fromIndex < 0 || fromIndex >= array.Length){
			throw new System.ArgumentOutOfRangeException("fromIndex", fromIndex, "Index must be in range [0,array.Length)");
		}

		if(toIndex < 0 || toIndex >= array.Length){
			throw new System.ArgumentOutOfRangeException("toIndex", toIndex, "Index must be in range [0,array.Length)");
		}

		var moving = array[fromIndex];
		int iterator = fromIndex > toIndex ? -1 : +1;
		for(int i = fromIndex; i != toIndex; i += iterator){
			array[i] = array[i + iterator];
		}
		array[toIndex] = moving;
	}

	public static T First<T>(T[] array, Func<T, bool> test) {
		for(int i=0;i<array.Length; i++) {
			if(test(array[i])) return array[i];
		}
		return default(T);
	}

	public static void ForEach<T> (
		this T[] array,
		Action<T, int> action
	) {
		for(int i=0;i<array.Length;i++) {
			action(array[i], i);
		}
	}

    /// 2D array operations
    public static void ForEach<T> (
        this T[,] array,
        int length0, int length1,
        Action<int, int, T> action
    ) {
        for(int x=0;x<length0; x++) {
            for (int y = 0; y < length1; y++)
            {
                action.Invoke(x, y, array[x, y]);
            }
        }
    }

    public static void ForEach<T>(
        this T[][] array,
        int length0, int length1,
        Action<int, int, T> action
    )
    {
        for (int x = 0; x < length0; x++)
        {
            for (int y = 0; y < length1; y++)
            {
                action.Invoke(x, y, array[x][y]);
            }
        }
    }
}
