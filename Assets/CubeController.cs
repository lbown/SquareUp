using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CubeController : MonoBehaviourPun
{
    private PhotonView PV;

    public bool inRotation = false;
    private Quaternion cubeRot;
    Vector2 actualXY, targetXY;
    GameObject gameManager;
    GameManager gm;
    public List<GameObject> lights;

    private void Start()
    {
        PV = gameObject.GetComponent<PhotonView>();
        cubeRot = gameObject.transform.rotation;
        gameManager = GameObject.FindWithTag("gm");
        gm = gameManager.GetComponent<GameManager>();
    }

    public void TransferOwnershipOfCube()
    {
        gameObject.GetComponent<PhotonView>().RequestOwnership();
        //gameManager.GetComponent<PhotonView>().RequestOwnership();
    }

    void Update()
    {
       if(PV.IsMine) { 
        if (inRotation)
            {
                gameObject.transform.rotation = cubeRot;

                targetXY = new Vector2(rubberBandX(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
                                       rubberBandY(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

                Vector2 diff = targetXY - actualXY;
                if (diff.magnitude < 0.1)
                {
                    actualXY = targetXY;
                }
                else
                {
                    actualXY += diff.normalized * 0.05f;
                }

                gameObject.transform.Rotate(transform.InverseTransformVector(Vector3.up), actualXY.x * 90);
                gameObject.transform.Rotate(transform.InverseTransformVector(Vector3.left), actualXY.y * 90);
                if (Input.GetButtonDown("Fire1") && actualXY.x == Mathf.Floor(actualXY.x) && actualXY.y == Mathf.Floor(actualXY.y))
                {
                    cubeRot = gameObject.transform.rotation;
                    targetXY = new Vector2(0, 0);
                    actualXY = new Vector2(0, 0);
                    StopRotation();
                }
            }
        }
    }

    public void StartRotation()
    {
        gm.pauseTime();
        inRotation = true;
        gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0, 20);
        foreach (GameObject light in lights)
        {
            light.transform.position += new Vector3(0, 0, 20);
        }
    }

    void StopRotation()
    {
        gm.unpauseTime();
        inRotation = false;
        gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0, -20);
        foreach (GameObject light in lights)
        {
            light.transform.position += new Vector3(0, 0, -20);
        }
    }

    float rubberBandX(float x, float y)
    {
        if (x > 0.85)
        {
            return 1;
        }
        if (x < -0.85)
        {
            return -1;
        }
        if ((y > 0.65 || y < -0.65) && x < 0.3 && x > -0.3)
        {
            return 0;
        }
        return x;
    }

    float rubberBandY(float x, float y)
    {
        if (y > 0.85)
        {
            return 1;
        }
        if (y < -0.85)
        {
            return -1;
        }
        if ((x > 0.65 || x < -0.65) && y < 0.3 && y > -0.3)
        {
            return 0;
        }
        return y;
    }
    
}
