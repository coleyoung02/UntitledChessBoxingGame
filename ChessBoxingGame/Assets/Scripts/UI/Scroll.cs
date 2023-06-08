using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    private float minY;
    private float timePassed;
    private void Start()
    {
        minY = this.gameObject.transform.position.y;
        timePassed = -1f;
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            int y = (int)Mathf.Round(Mathf.Max(minY, this.gameObject.transform.position.y - 1));
            this.gameObject.transform.position = new Vector2(this.gameObject.transform.position.x, y);
            timePassed = -1f;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            int y = (int)Mathf.Round(Mathf.Min(35f+minY, this.gameObject.transform.position.y + 1));
            this.gameObject.transform.position = new Vector2(this.gameObject.transform.position.x, y);
            timePassed = -1f;
        }
        else if (timePassed > 1f)
        {
            timePassed = .9f;
            float y = Mathf.Min(35f + minY, this.gameObject.transform.position.y + .125f);
            this.gameObject.transform.position = new Vector2(this.gameObject.transform.position.x, y);
        }
        timePassed += Time.deltaTime;
    }
}
