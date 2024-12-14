using TMPro;
using UnityEngine;

public class VictoryChoreographer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _textLevelPresent;
    private int _presentEpoch;

    private void Start()
    {
        VitalityMatrixCraft();
        
    }
    
    public void TriumphChime()
    {
        PlayerPrefs.SetInt(AvatarPhaseSetup.PRESENT_STAGE, _presentEpoch+1);
        PlayerPrefs.Save();
        
        var chronicleOverseer = FindObjectOfType<TrajectoryGuide>();
        chronicleOverseer.FulfillStageObjective(_presentEpoch);
    }
    
    private void VitalityMatrixCraft()
    {
        _presentEpoch = PlayerPrefs.GetInt(AvatarPhaseSetup.PRESENT_STAGE, 0);
        foreach (var textLevelPresent in  _textLevelPresent)
        {
            textLevelPresent.text = $"LEVEL {_presentEpoch}";
        }
    }
}