using UnityEngine;
using UnityEngine.UI;

public class SimplePopup : MonoBehaviour
{
    [Header("Composants UI")]
    public RawImage contentImage;
    public Button closeButton;

    private PopupManager popupManager;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        popupManager = FindObjectOfType<PopupManager>();

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePopup);
        }
    }

    void ClosePopup()
    {
        if (popupManager != null)
        {
            popupManager.RemovePositionFromList(rectTransform.anchoredPosition);
        }
        Destroy(gameObject);
    }

    public void SetupPopup(Texture texture)
    {
        if (contentImage != null && texture != null)
        {
            contentImage.texture = texture;
        }
    }
}