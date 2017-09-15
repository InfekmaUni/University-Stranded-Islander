using UnityEngine;
using System.Collections;

/*Author: Ka Ming Li*/
public class MiniMapFollow : MonoBehaviour
{
    public Transform target;
    private float heightOffSet = 50;
    // move minimap according to main player camera
    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y + heightOffSet, target.position.z);
    }
}
