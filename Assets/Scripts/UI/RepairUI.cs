using UnityEngine;
using UnityEngine.UI;

public class RepairUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    public Image repairFillImage; 

    public void SetMaxHeal(int heal)
    {
        slider.maxValue = heal;
        slider.value = heal;
    }

    public void SetHeal(int heal)
    {
        slider.value = heal;
    }
}