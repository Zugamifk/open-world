using UnityEngine;
using System.Collections;

public interface IMeshGenerator
{
    string Name { get; }
    Mesh Generate();
}
