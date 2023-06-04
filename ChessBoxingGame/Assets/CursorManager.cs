using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    //[SerializeField] private Texture2D cursorMain;
    //[SerializeField] private Texture2D cursorLight;

    private Vector2 cursorHotspot;

    [SerializeField] private GameObject cursorObj;
    [SerializeField] private RectTransform cursorObjRect;

    // Start is called before the first frame update
    void Start()
    {
        cursorHotspot = new Vector2(cursorObjRect.rect.width / 2, cursorObjRect.rect.height / 2);
        //Cursor.SetCursor(cursorMain, cursorHotspot, CursorMode.ForceSoftware);
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorObj.transform.position = cursorPos;
    }

    /*
    public void OnButtonCursorEnter()
    {
        Cursor.SetCursor(cursorLight, cursorHotspot, CursorMode.ForceSoftware);
    }

    public void OnButtonCursorExit()
    {
        Cursor.SetCursor(cursorMain, cursorHotspot, CursorMode.ForceSoftware);
    }
    */
}
