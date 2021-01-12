using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLoad : MonoBehaviour
{
    public static AsyncLoad instance;
    [SerializeField]
    private bool autoLoad;
    public UnityEngine.AsyncOperation async;
    public int level;
    void Start()
    {
        
        instance = this;

            StartCoroutine(loadAsyncScene());

        level = SceneManager.GetActiveScene().buildIndex;
    }

    public IEnumerator loadAsyncScene()
    {
        yield return new WaitForSeconds(0.1f);
        async = SceneManager.LoadSceneAsync(level);
        async.allowSceneActivation = false;

        yield return async;
        
    }

    public void switchChangeLevel()
    {
        async.allowSceneActivation = true;
    }
}
