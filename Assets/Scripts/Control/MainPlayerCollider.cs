using UnityEngine;
using System.Collections;

public class MainPlayerCollider : MonoBehaviour {

    void OnDrawGizmos() {
        Gizmos.color = Colorx.lightgreen;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
