using UnityEngine;

public class VictoryChoreographer : MonoBehaviour
{ 
    private int _presentEpoch;

    private void Awake()
    {
        VitalityMatrixCraft();
    }
    
    public void TriumphChime()
    {
        PlayerPrefs.SetInt(AvatarPhaseSetup.PRESENT_STAGE, _presentEpoch);
        PlayerPrefs.Save();
        
        var chronicleOverseer = FindObjectOfType<TrajectoryGuide>();
        chronicleOverseer.FulfillStageObjective(_presentEpoch);
    }
    
    private void VitalityMatrixCraft()
    {
        _presentEpoch = PlayerPrefs.GetInt(AvatarPhaseSetup.PRESENT_STAGE, 0);
    }
}