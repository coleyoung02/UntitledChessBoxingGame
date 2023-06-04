using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoxingText : MonoBehaviour
{
    [SerializeField] Image KO;
    [SerializeField] AudioClip KOBell;
    [SerializeField] AudioSource source;


    public void onKO()
    {
        KO.gameObject.SetActive(true);
        source.clip = KOBell;
        source.Play();
        StartCoroutine(showKO());
    }


    IEnumerator showKO()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("EndScreen");
    }
}
