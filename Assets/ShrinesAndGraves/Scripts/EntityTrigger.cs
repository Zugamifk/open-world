using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class EntityTrigger : MonoBehaviour
    {

        void OnTriggerEnter2D(Collider2D other) {
            var tr = other.GetComponent<TriggerReceiver>();
            if (tr != null)
            {
                tr.OnEnterTrigger(this);
            }
        }
    }
}