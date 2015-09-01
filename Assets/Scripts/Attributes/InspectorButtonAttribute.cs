using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InspectorButtonAttribute : PropertyAttribute {

	public List<string> methodNames = new List<string>();
    public List<string> buttonLabels = new List<string>();

    public InspectorButtonAttribute(string methodName) {
		this.methodNames.Add(methodName);
	}

	public InspectorButtonAttribute(string methodName, string label)
	: this(methodName) {
		this.buttonLabels.Add(label);
	}

	public InspectorButtonAttribute(string methodName, string label, params string[] others)
	: this(methodName, label) {
		for(int i = 0;i<others.Length/2;i+=2) {
			if(others[i]==null) {
                throw new System.ArgumentException(others[i], "Can not give null method name!");
            } else {
                methodNames.Add(others[i]);
            }
			if(i+1<others.Length) {
				buttonLabels.Add(others[i+1]);
			} else {
				buttonLabels.Add(null);
			}
		}
	}
}
