using UnityEngine;
using UnityEngine.InputSystem;
public class MenuControl : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public bool showMenu = false;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;  // Ensure the panel is visible initially
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            showMenu = !showMenu;
            if (showMenu)
            {
                ShowPanel();
            }
            else
            {
                HidePanel();
            }
        }
    }

    public void HidePanel()
    {
        canvasGroup.alpha = 0f;  // Make the panel invisible
        canvasGroup.interactable = false;  // Disable interactions
        canvasGroup.blocksRaycasts = false;  // Prevent raycasts
    }

    public void ShowPanel()
    {
        canvasGroup.alpha = 1f;  // Make the panel visible
        canvasGroup.interactable = true;  // Enable interactions
        canvasGroup.blocksRaycasts = true;  // Allow raycasts
    }
}

