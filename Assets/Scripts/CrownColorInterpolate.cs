using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownColorInterpolate : MonoBehaviour
{
    private MeshRenderer[] crownRenderers;
    [SerializeField] [Range(0f, 1f)] float lerpTime;
    [SerializeField] private List<Color> myColors;
    [SerializeField] private float t, modifier;
    private int colorIndex, len;
    // Start is called before the first frame update
    void Awake()
    {
        StartCrown();
    }

    // Update is called once per frame
    void Update()
    {
        LerpThroughColors();
        ConstantlyRotate();
    }

    private void LerpThroughColors()
    {
        for (int i = 0; i < crownRenderers.Length; i++)
        {
            crownRenderers[i].material.SetColor("_BaseColor", Color.Lerp(crownRenderers[i].material.GetColor("_BaseColor"), myColors[(colorIndex + 1) % len], lerpTime * Time.deltaTime));
        }
        t = Mathf.Lerp(t, 1f, lerpTime * Time.deltaTime);
        if (t > .9f)
        {
            t = 0f;
            colorIndex++;
            colorIndex = colorIndex % len;
            Debug.Log("Color Index: " + colorIndex);
        }
    }

    private void ConstantlyRotate()
    {
        Vector3 localUp = transform.TransformVector(new Vector3(0, 1, 0));
        transform.Rotate(localUp, 360 * Time.deltaTime * modifier);
    }

    public void SetCrownRenderers(bool rendererEnabled)
    {
        for (int i = 0; i < crownRenderers.Length; i++)
        {
            crownRenderers[i].enabled = rendererEnabled;
        }
    }

    public void StartCrown()
    {
        crownRenderers = GetComponentsInChildren<MeshRenderer>();
        myColors = new List<Color>();
        myColors.Add(new Color32(212, 175, 55, 255));
        foreach (int mat in PlayerInfo.PI.alreadySelectedMaterials)
        {
            myColors.Add(PlayerInfo.PI.totalMaterials[mat].GetColor("_BaseColor"));
        }
        colorIndex = 0;
        t = 0f;
        len = myColors.Count;
    }
}
