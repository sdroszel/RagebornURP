using System.Collections;
using UnityEngine;

public class CompleteGame : MonoBehaviour
{
    [SerializeField] private GameObject gameCompleteText;

    private void Awake()
    {
        gameCompleteText.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(Return());
    }

    private IEnumerator Return()
    {
        gameCompleteText.SetActive(true);

        yield return new WaitForSeconds(3);

        SceneManagerScript.instance.LoadMainMenu();
    }
}
