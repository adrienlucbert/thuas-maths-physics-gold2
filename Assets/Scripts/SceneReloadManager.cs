using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneReloadManager : MonoBehaviour
{
    public void ReloadScene()
    {
        StartCoroutine(this.ReloadSceneAsync(SceneManager.GetActiveScene().name));
    }

    private IEnumerator ReloadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
            yield return null;
    }
}
