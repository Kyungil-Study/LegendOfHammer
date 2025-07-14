using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobTang : MonoBehaviour
{
    public ClampedInt hp;

    private void Start()
    {
        hp = 3 + hp;
        hp = hp + 3;
    }
}
