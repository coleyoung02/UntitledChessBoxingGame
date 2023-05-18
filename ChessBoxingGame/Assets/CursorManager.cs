using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorMain;
    [SerializeField] private Texture2D cursorLight;

    private Vector2 cursorHotspot;

    // Start is called before the first frame update
    void Start()
    {
        cursorHotspot = new Vector2(cursorMain.width / 2, cursorMain.height / 2);
        Cursor.SetCursor(cursorMain, cursorHotspot, CursorMode.ForceSoftware);
    }

    public void OnButtonCursorEnter()
    {
        Cursor.SetCursor(cursorLight, cursorHotspot, CursorMode.ForceSoftware);
    }

    public void OnButtonCursorExit()
    {
        Cursor.SetCursor(cursorMain, cursorHotspot, CursorMode.ForceSoftware);
    }
}
