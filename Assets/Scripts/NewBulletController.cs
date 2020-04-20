using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NewBulletController : MonoBehaviour
{
    // Start is called before the first frame update
    private float life;

    public int whoShotMe;

    public Vector3 impulse;

    void Start()
    {
        life = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        life += Time.deltaTime;
        if (life > 5)
        {
            Destroy(gameObject);
        }
        Vector3 vel = gameObject.GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().velocity = new Vector3(vel.x, vel.y, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

}
