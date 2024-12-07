using UnityEngine;

public class EgressNavigator : MonoBehaviour
{ 
    public void QuestUmbral()
    {
#if UNITY_EDITOR
        
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}