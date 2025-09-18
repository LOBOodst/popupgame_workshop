using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AdminPopup : MonoBehaviour
{
    [Header("Composants UI")]
    public Text adminIdText;
    public InputField codeInput;
    public Button submitButton;

    private string correctCode;
    private PopupManager popupManager;

    private Dictionary<string, string> adminCodes = new Dictionary<string, string>()
    {
        {"pierrot.mann", "PyroDino1732"},
        {"zdirector", "Hollandais_Vollant"},
        {"Xxstage.hierxX", "ousontlestoilettes"}
    };

    private string currentAdminId;

    void Start()
    {
        popupManager = FindObjectOfType<PopupManager>();

        int randomIndex = Random.Range(0, adminCodes.Count);
        currentAdminId = new List<string>(adminCodes.Keys)[randomIndex];
        correctCode = adminCodes[currentAdminId];

        if (adminIdText != null)
            adminIdText.text = currentAdminId;

        if (submitButton != null)
            submitButton.onClick.AddListener(CheckCode);
    }

    void CheckCode()
    {
        if (codeInput == null) return;

        string playerInput = codeInput.text.ToLower().Trim();

        if (playerInput == correctCode.ToLower())
        {
            // Code correct → ferme le popup et supprime 5 popups aléatoires
            Debug.Log("Code correct pour Admin " + currentAdminId);
            if (popupManager != null)
            {
                popupManager.RemoveRandomPopups(5); // Supprime 5 popups aléatoires
            }
            ClosePopup();
        }
        else
        {
            // Code incorrect → fait apparaître plus de popups
            Debug.Log("Code incorrect pour Admin " + currentAdminId);

            if (popupManager != null)
            {
                popupManager.SpawnExtraPopups(); // Fait apparaître plus de popups
            }

            // Vide le champ de saisie et garde le focus
            codeInput.text = "";
            codeInput.Select();
            codeInput.ActivateInputField();
        }
    }

    void ClosePopup()
    {
        if (popupManager != null)
            popupManager.AdminPopupClosed();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (popupManager != null)
        {
            popupManager.AdminPopupClosed();
        }
    }
}