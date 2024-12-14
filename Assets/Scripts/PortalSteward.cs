using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSteward : MonoBehaviour
{
    public void LoadLimboRealm()
    {
        LoadSceneByName(AvatarPhaseSetup.PIXEL_ADVENTURE);
    }
    
    public void LoadVoxelOdysseyRealm()
    {
        LoadSceneByName(AvatarPhaseSetup.INTERIM_DOMAIN);
    }
    
    public void LoadsSceneBonusGame()
    {
        LoadSceneByName(AvatarPhaseSetup.BONUS_GAME);
    }
    
    private void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}