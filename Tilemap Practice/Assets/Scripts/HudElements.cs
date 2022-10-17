using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudElements : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI blueManaCap;
    [SerializeField] TextMeshProUGUI redManaCap;
    [SerializeField] TextMeshProUGUI whiteManaCap;
    [SerializeField] TextMeshProUGUI blackManaCap;
    [SerializeField] TextMeshProUGUI greenManaCap;

    [SerializeField] TextMeshProUGUI whiteMana;
    [SerializeField] TextMeshProUGUI blackMana;
    [SerializeField] TextMeshProUGUI blueMana;
    [SerializeField] TextMeshProUGUI greenMana;
    [SerializeField] TextMeshProUGUI redMana;

    public void UpdateHudElements(PlayerResources playerResources)
    {
        blueMana.text = playerResources.blueMana.ToString();
            
    }
}
