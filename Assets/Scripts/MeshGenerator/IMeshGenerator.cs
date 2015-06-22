using UnityEngine;
using System.Collections;

namespace MeshGenerator
{
    interface IMeshGenerator
    {
        string Name { get; }
        Mesh Generate();
    }
}
