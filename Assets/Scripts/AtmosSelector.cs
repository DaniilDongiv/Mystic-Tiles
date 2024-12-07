using UnityEngine;
using UnityEngine.UI;

public class AtmosSelector : MonoBehaviour
{
    [SerializeField] private Image previewImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite[] previewOptions;
    [SerializeField] private Sprite[] backgroundOptions;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button gameSaveButton;

    private int currentBackgroundIndex = 0;

    private void Start()
    {
        if (PlayerPrefs.HasKey("SelectedBackground"))
        {
            currentBackgroundIndex = PlayerPrefs.GetInt("SelectedBackground");
        }

        RevisePreviewPane();
        
        nextButton.onClick.AddListener(BackdropAdvance);
        prevButton.onClick.AddListener(BackdropRetro);
        gameSaveButton.onClick.AddListener(RetainBackdrop);
    }
    
    private void BackdropAdvance()
    {
        currentBackgroundIndex = (currentBackgroundIndex + 1) % previewOptions.Length;
        RevisePreviewPane();
    }
    
    private void BackdropRetro()
    {
        currentBackgroundIndex = (currentBackgroundIndex - 1 + previewOptions.Length) % previewOptions.Length;
        RevisePreviewPane();
    }
    
    private void RevisePreviewPane()
    {
        previewImage.sprite = previewOptions[currentBackgroundIndex];
    }
    
    private void RetainBackdrop()
    {
        PlayerPrefs.SetInt("SelectedBackground", currentBackgroundIndex);
        PlayerPrefs.Save();
        
        backgroundImage.sprite = backgroundOptions[currentBackgroundIndex];
    }
}
