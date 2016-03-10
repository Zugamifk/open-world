using UnityEngine;
using System.Collections;

public interface IDebuggable {
    string name { get; }
    void GetDebugMessageArgs(out string format, out System.Func<object>[] args);
}
