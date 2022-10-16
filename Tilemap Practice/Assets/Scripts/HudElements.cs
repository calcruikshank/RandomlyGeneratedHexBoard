using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudElements : MonoBehaviour
{
    [SerializeField] TextMeshPro blueManaCap;
    [SerializeField] TextMeshPro redManaCap;
    [SerializeField] TextMeshPro whiteManaCap;
    [SerializeField] TextMeshPro blackManaCap;
    [SerializeField] TextMeshPro greenManaCap;

    [SerializeField] TextMeshPro whiteMana;
    [SerializeField] TextMeshPro blackMana;
    [SerializeField] TextMeshPro blueMana;
    [SerializeField] TextMeshPro greenMana;
    [SerializeField] TextMeshPro redMana;

    public void UpdateHudElements(PlayerResources playerResources)
    {
        blueMana.text = playerResources.blueMana.ToString();
            
    }
}
