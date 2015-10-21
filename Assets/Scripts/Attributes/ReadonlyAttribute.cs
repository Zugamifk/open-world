using UnityEngine;
using System.Collections;

public class ReadonlyAttribute : PropertyAttribute {

	public string label = string.Empty;

    public ReadonlyAttribute() {	}

	public ReadonlyAttribute(string label) {
		this.label = label;
	}

}
