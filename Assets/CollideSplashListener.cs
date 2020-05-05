using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideSplashListener : MonoBehaviour
{
    private GameObject on;
    public LayerMask canvasMask;
    public LayerMask groundMask;

    private void OnParticleCollision(GameObject other)
    {
        transform.localScale += new Vector3(0.2f, 0, 0.2f);

        bool offEdge = false;

        foreach (Transform t in transform)
        {
            if (!Physics.CheckSphere(t.position, 0.01f, canvasMask) && !Physics.CheckSphere(t.position, 0.01f, groundMask))
            {
                offEdge = true;
            }
        }
        if(offEdge)
        {
            transform.localScale -= new Vector3(0.2f, 0, 0.2f);
        } 
    }
}
