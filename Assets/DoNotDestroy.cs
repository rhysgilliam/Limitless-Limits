using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    private void Awake()
    {
        var musicObj = GameObject.FindGameObjectsWithTag("Game_Music");
        if (musicObj.Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
