using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    [SerializeField] private Image fuelImage;

    private void UpdateFuelTextMesh()
    {
        fuelImage.fillAmount = Lander.Instance.fuelAmountNormalized();
    }


    private void Update()
    {
        UpdateFuelTextMesh();
    }
}
