using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudElements : MonoBehaviour
{
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
    }
}
