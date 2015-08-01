using UnityEngine;
using System.Collections;
using System;

public static class Typex {

	public static bool ImplementsInterface(this Type type, Type ifaceType ) {
		Type[] intf = type.GetInterfaces();
		for ( int i = 0; i < intf.Length; i++ ) {
			if ( intf[ i ] == ifaceType ) {
				return true;
			}
		}
		return false;
	}
}
