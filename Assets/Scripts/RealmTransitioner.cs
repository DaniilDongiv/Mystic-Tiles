using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RealmTransitioner : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(StreamStageAsync(AvatarPhaseSetup.PRIMARY_DIMENSION));
    }
    
    private IEnumerator StreamStageAsync(string sceneName)
    {
        var loadOperation = SceneManager.LoadSceneAsync(sceneName);
        loadOperation.allowSceneActivation = false;
        
        while (!loadOperation.isDone)
        {
            if (loadOperation.progress >= 0.9f)
            {
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}