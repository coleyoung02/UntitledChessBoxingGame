using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    private float minY;
    private void Start()
    {
        minY = this.gameObject.transform.position.y;
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            float y = Mathf.Max(minY, this.gameObject.transform.position.y - 1);
            this.gameObject.transform.position = new Vector2(this.gameObject.transform.position.x, y);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            float y = Mathf.Min(60f+minY, this.gameObject.transform.position.y + 1);
            this.gameObject.transform.position = new Vector2(this.gameObject.transform.position.x, y);
        }
    }
}
