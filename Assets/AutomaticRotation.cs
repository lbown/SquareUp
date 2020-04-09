using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticRotation : MonoBehaviour
{
    [SerializeField] private float modifier;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localUp = transform.TransformVector(new Vector3 (1, 1, 1));
        transform.Rotate(localUp, 360 * Time.deltaTime * modifier);
    }
}
