using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SimpleSceneTransition : MonoBehaviour
{
    public CanvasGroup transitionAlpha;
    public GameObject[] transitionPanels;

    public void Transition(string nextLevel, int panelIndex = 0 )
    {
        foreach (GameObject tp in transitionPanels)
        {
            tp.SetActive(false);
        }
        transitionPanels[panelIndex].SetActive(true);

        transitionAlpha.blocksRaycasts = true;
        transitionAlpha.DOFade(1, 0.1f).SetUpdate(true).OnComplete(() => StartCoroutine(LoadScene(nextLevel)));
    }
  
    IEnumerator LoadScene(string nextLevel)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextLevel);
        // Kill all tweens
        DOTween.KillAll();

        while (!op.isDone)
        {
            yield return null;
        }
        

        //do Transition
        transitionAlpha.DOFade(0, 0.1f).SetUpdate(true);
        transitionAlpha.blocksRaycasts = false;
    }
}
