using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTransparency : MonoBehaviour
{
    Renderer thisRenderer;
    float originalTransparency;
    private void Awake()
    {
        thisRenderer = this.gameObject.GetComponent<Renderer>();
    }
    public void ChangeTransparent(int v)
    {
        Color32 col = thisRenderer.material.GetColor("_Color");
        col.a = 50;
        this.gameObject.GetComponent<Renderer>().material.SetColor("_Color", col);
    }

    public void SetOpaque()
    {
        Color32 col = thisRenderer.material.GetColor("_Color");
        col.a = 255;
        this.gameObject.GetComponent<Renderer>().material.SetColor("_Color", col);
    }
}
