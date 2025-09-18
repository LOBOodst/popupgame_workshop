using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RamUsage : MonoBehaviour
{
    public Slider ramSlider;
    public float ramUsage = 0.33f;
    [SerializeField] private TextMeshProUGUI ramUsageText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ramSlider.value = ramUsage;
    }

    // Update is called once per frame
    void Update()
    {
        ramSlider.value = ramUsage;
        ramUsageText.text = "Ram Usage : " + ramUsage.ToString() + " gB / 4,0 gB";
    }
}
