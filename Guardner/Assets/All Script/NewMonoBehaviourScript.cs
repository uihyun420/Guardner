using UnityEngine;


public class NewMonoBehaviourScript : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }

    void Update()
    {
        
    }
}
