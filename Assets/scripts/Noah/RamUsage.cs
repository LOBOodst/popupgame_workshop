using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class RamUsage : MonoBehaviour
{
    public Slider ramSlider;
    public float ramUsage = 0.33f;
    [SerializeField] private TextMeshProUGUI ramUsageText;
    [SerializeField] private Image sliderFill;
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

        if (ramSlider.value >= 3.5f) sliderFill.color = Color.red;
        else if( ramSlider.value >= 2.0f) sliderFill.color = Color.yellow;
        else sliderFill.color = Color.green;
    }
}
