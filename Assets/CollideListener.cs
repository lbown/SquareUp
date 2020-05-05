using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideListener : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject bloodObj;
    public LayerMask canvasMask;
    public LayerMask groundMask;
    public GameObject cubeTransform;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        ParticlePhysicsExtensions.GetCollisionEvents(other.GetComponent<ParticleSystem>(), gameObject, collisionEvents);

        foreach(ParticleCollisionEvent p in collisionEvents) {
            GameObject blood = Instantiate(bloodObj, cubeTransform.transform);
            blood.transform.position = p.intersection;
            blood.transform.rotation = Quaternion.FromToRotation(Vector3.up, p.normal);
            bool offEdge = false;
            foreach(Transform t in blood.transform)
            {
                if (!Physics.CheckSphere(t.position, 0.01f, canvasMask) && !Physics.CheckSphere(t.position, 0.01f, groundMask))
                {
                    offEdge = true;
                }
            }

            if (offEdge)
            {
               Destroy(blood);
            } else
            {
                blood.SetActive(true);
            }

        }
    }
}
