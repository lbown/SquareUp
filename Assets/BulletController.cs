using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update
    private float life;
    void Start()
    {
        life = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        life += Time.deltaTime;
        if(life > 5)
        {
            Destroy(gameObject);
        }
    }
}
