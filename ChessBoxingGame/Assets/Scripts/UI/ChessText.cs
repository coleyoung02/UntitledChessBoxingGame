using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChessText : MonoBehaviour
{
    [SerializeField] Image check;
    [SerializeField] Image mate;
    [SerializeField] AudioClip checkSound;
    [SerializeField] AudioClip mateSound;
    [SerializeField] AudioSource source;


    public void onCheck()
    {
        check.gameObject.SetActive(true);
        source.clip = checkSound;
        source.Play();
        StartCoroutine(hideCheck());
    }

    public void onMate()
    {
        source.clip = mateSound;
        source.Play();
        check.gameObject.SetActive(true);
        StartCoroutine(waitForMate());

    }

    IEnumerator hideCheck()
    {
        yield return new WaitForSeconds(1f);
        check.gameObject.SetActive(false);
    }

    IEnumerator waitForMate()
    {
        yield return new WaitForSeconds(.65f);
        mate.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("EndScreen");
    }
}
