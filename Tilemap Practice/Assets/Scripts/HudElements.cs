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
        blackMana.text = playerResources.blackMana.ToString();
        whiteMana.text = playerResources.whiteMana.ToString();
        redMana.text = playerResources.redMana.ToString();
        greenMana.text = playerResources.greenMana.ToString();

        blueManaCap.text = playerResources.blueManaCap.ToString();
        whiteManaCap.text = playerResources.whiteManaCap.ToString();
        redManaCap.text = playerResources.redManaCap.ToString();
        blackManaCap.text = playerResources.blackManaCap.ToString();
        greenManaCap.text = playerResources.greenManaCap.ToString();
    }
}
