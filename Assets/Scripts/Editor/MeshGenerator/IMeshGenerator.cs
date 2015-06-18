using UnityEngine;
using System.Collections;

interface IMeshGenerator {
	string Name {get;}
	Mesh Generate();
}
