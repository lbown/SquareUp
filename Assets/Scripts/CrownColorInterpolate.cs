using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownColorInterpolate : MonoBehaviour
{
    private MeshRenderer thisRenderer;
    [SerializeField] [Range(0f, 1f)] float lerpTime;
    [SerializeField] List<Color> myColors;
    private float t;
    private int colorIndex, len;
    // Start is called before the first frame update
    void Start()
    {
        thisRenderer = GetComponent<MeshRenderer>();
        foreach (Material material in PlayerInfo.PI.totalMaterials)
        {
            myColors.Add(material.color);
        }
        colorIndex = 0;
        t = 0f;
        len = myColors.Count;
    }

    // Update is called once per frame
    void Update()
    {
        thisRenderer.material.color = Color.Lerp(thisRenderer.material.color, myColors[colorIndex], lerpTime*Time.deltaTime);
        t = Mathf.Lerp(t, 1f, lerpTime*Time.deltaTime);
        if (t > .9f)
        {
            t = 0f;
            colorIndex++;
            colorIndex = (colorIndex >= len) ? 0 : colorIndex;
        }
    }
}
