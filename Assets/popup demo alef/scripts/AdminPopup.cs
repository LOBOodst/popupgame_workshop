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
            Debug.Log("Code correct pour Admin " + currentAdminId);
            if (popupManager != null)
            {
                popupManager.RemoveRandomPopups(5);
            }
            ClosePopup();
        }
        else
        {
            Debug.Log("Code incorrect pour Admin " + currentAdminId);

            if (popupManager != null)
            {
                popupManager.SpawnExtraPopups();
            }

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