using UnityEngine;
using UnityEngine.SceneManagement;

/* Control screening fading animation, when changing game scences.*/
public class SceneFadeAnimation : MonoBehaviour {

    int _sceneIndexToLoad;

    //Set _sceneIndexToLoad to index of the scene we want to load, when the fade animation is finished. 
    public void fadeTolevel(int buildIndex) {
        _sceneIndexToLoad = buildIndex;
        GetComponent<Animator>().SetTrigger("Fade Out");
    }

    //Use as a animation event at the end of the fade animation. 
    public void onFadeComplete() {
        SceneManager.LoadScene(_sceneIndexToLoad);
    }
}
