using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovingPopup : MonoBehaviour
{
    [Header("Composants UI")]
    public RawImage contentImage;
    public Button closeButton;

    [Header("Paramètres de Mouvement")]
    public int maxMoves = 3;
    public float moveDuration = 0.5f;
    public float timeBetweenMoves = 1f;
    public Vector2 moveAreaMin = new Vector2(-400, -200);
    public Vector2 moveAreaMax = new Vector2(400, 200);

    private int currentMoveCount = 0;
    private bool canClose = false;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private PopupManager popupManager;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        popupManager = FindObjectOfType<PopupManager>();

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(TryClosePopup);
            closeButton.interactable = false;
        }

        StartCoroutine(MovementRoutine());
    }

    IEnumerator MovementRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < maxMoves; i++)
        {
            Vector2 targetPosition = GetRandomPosition();
            yield return StartCoroutine(MoveToPosition(targetPosition));

            if (i < maxMoves - 1)
            {
                yield return new WaitForSeconds(timeBetweenMoves);
            }

            currentMoveCount++;
        }

        canClose = true;
        if (closeButton != null)
        {
            closeButton.interactable = true;
        }
    }

    IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            t = 1f - Mathf.Pow(1f - t, 3f); // Courbe d'easing

            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }

    Vector2 GetRandomPosition()
    {
        return new Vector2(
            Random.Range(moveAreaMin.x, moveAreaMax.x),
            Random.Range(moveAreaMin.y, moveAreaMax.y)
        );
    }

    void TryClosePopup()
    {
        if (canClose)
        {
            ClosePopup();
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

    public void SetInitialPosition(Vector2 position)
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
            originalPosition = position;
        }
    }
}