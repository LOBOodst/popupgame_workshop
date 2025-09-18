using UnityEngine;
using UnityEngine.UI;

public class SimplePopup : MonoBehaviour
{
    [Header("UI Components")]
    public RawImage contentImage;
    public Button closeButton;

    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePopup);
        }
        else
        {
            Debug.LogError("Close button is not assigned!");
        }
    }

    void ClosePopup()
    {
        Destroy(gameObject);
    }

    public void SetupPopup(Texture texture)
    {
        if (contentImage != null && texture != null)
        {
            contentImage.texture = texture;
        }
    }

    // Método para forçar a posição (chamado pelo manager)
    public void ForcePosition(Vector2 position)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
            Debug.Log($"Popup forced to position: {position}");
        }
    }
}