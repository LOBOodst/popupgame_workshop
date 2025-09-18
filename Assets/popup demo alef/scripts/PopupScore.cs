using UnityEngine;

public class PopupScore : MonoBehaviour
{
    public PopupManager popupManager;
    public int scoreValue;
    public string popupType;

    void Start()
    {
        // Enregistre les popups sp�ciaux
        if (popupType == "AdminPopup" || popupType == "MathPopup")
        {
            popupManager?.RegisterSpecialPopup(popupType, true);
        }
    }

    void OnDestroy()
    {
        if (popupManager != null)
        {
            // D�senregistre les popups sp�ciaux
            if (popupType == "AdminPopup" || popupType == "MathPopup")
            {
                popupManager.RegisterSpecialPopup(popupType, false);
            }

            popupManager.AddScore(scoreValue, popupType);
        }
    }
}