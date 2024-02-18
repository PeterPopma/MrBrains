using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);

        foreach (var collider in colliders)
        {
            Character character = collider.GetComponent<Character>();
            if (character != null)
            {
                character.ReachedTarget();
            }
        }
    }
}
