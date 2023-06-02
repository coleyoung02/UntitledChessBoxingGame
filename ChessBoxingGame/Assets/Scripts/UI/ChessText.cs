using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessText : MonoBehaviour
{
    [SerializeField] Image check;
    [SerializeField] Image mate;

    public void onCheck()
    {
        check.gameObject.SetActive(true);
        StartCoroutine(hideCheck());
    }

    public void onMate()
    {
        check.gameObject.SetActive(true);
        mate.gameObject.SetActive(true);

    }

    IEnumerator hideCheck()
    {
        yield return new WaitForSeconds(1f);
        check.gameObject.SetActive(false);
    }
}
