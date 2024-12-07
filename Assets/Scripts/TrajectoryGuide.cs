using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrajectoryGuide : MonoBehaviour
{
    public static TrajectoryGuide AppGlobalInstance;

    [SerializeField] private GameObject[] _levelOptionButtons;
    [SerializeField] private Button _chooseButton;

    private int _totalStagesCount = 60;
    private int _chosenLevelIndex = 1;

    private void Start()
    {
        
        if (AppGlobalInstance == null)
        {
            AppGlobalInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        ConfigurePlayfields();
        RefreshZoneSelectors();
        _chooseButton.interactable = false;
    }
    
    public void SelectEpoch(int levelIndex)
    {
        _chosenLevelIndex = levelIndex;
        _chooseButton.interactable = true;
    }

    public void CommenceZone()
    {
        if (_chosenLevelIndex == -1) return;
        
        PlayerPrefs.SetInt(AvatarPhaseSetup.PRESENT_STAGE, _chosenLevelIndex);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(AvatarPhaseSetup.PIXEL_ADVENTURE);
    }

    public void FulfillStageObjective(int levelIndex)
    {
        PlayerPrefs.SetInt("StageCleared" + levelIndex, 1);
        PlayerPrefs.Save();
        
        if (levelIndex < _totalStagesCount - 1)
        {
            PlayerPrefs.SetInt(AvatarPhaseSetup.PROGRESSION_PATHWAY + levelIndex, 1);
            PlayerPrefs.Save();
        }
        
        RefreshZoneSelectors();
    }

    private void ConfigurePlayfields()
    {
        for (var loopStepCounter = 0; loopStepCounter < _totalStagesCount; loopStepCounter++)
        {
            if (PlayerPrefs.GetInt(AvatarPhaseSetup.PROGRESSION_PATHWAY + loopStepCounter, -1) != -1) continue;
            PlayerPrefs.SetInt(AvatarPhaseSetup.PROGRESSION_PATHWAY + loopStepCounter,
                loopStepCounter == 0 ? 1 : 0);

            PlayerPrefs.SetInt("StageCleared" + loopStepCounter, 0);
        }
        PlayerPrefs.Save();
    }

    private void RefreshZoneSelectors()
    {
        for (var iterationCounter = 0; iterationCounter < _totalStagesCount; iterationCounter++)
        {
            if (_levelOptionButtons[iterationCounter] == null) continue;
            var isLevelUnlocked = PlayerPrefs.GetInt(AvatarPhaseSetup.PROGRESSION_PATHWAY + iterationCounter, 0) == 1;
            var levelButton = _levelOptionButtons[iterationCounter].GetComponent<Button>();
            levelButton.interactable = isLevelUnlocked;
        }
    }
}
