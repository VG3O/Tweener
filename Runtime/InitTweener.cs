using UnityEngine;
using UnityEngine.SceneManagement;
using VG.Tweener;

public class InitTweener : MonoBehaviour
{
    /*
        Attach this script to an empty gameobject which will self clean after tweener is done initializing 
        
        Make sure to copy the attached game object between scenes, Tweener will not init more than once.
    */
    private void Awake()
    {
        Tweener.Init();
        Destroy(gameObject);
    }
}
