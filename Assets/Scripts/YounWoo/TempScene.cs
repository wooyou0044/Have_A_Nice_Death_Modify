using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempScene : MonoBehaviour
{
    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 1.68f, -10);
    }
}
