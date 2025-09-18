using UnityEngine;

public class SwitchPc : MonoBehaviour
{
    [SerializeField] GameObject pcPanel;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //check if the Pc Panel is active or not and activate or deactive it 
            if(pcPanel.gameObject.activeInHierarchy)
            {
                pcPanel.gameObject.SetActive(false);
            }
            else
            {
                pcPanel.gameObject.SetActive(true);
            }
        }
    }
}
