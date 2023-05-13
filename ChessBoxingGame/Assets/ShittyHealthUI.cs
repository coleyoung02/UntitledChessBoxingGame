using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ShittyHealthUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Fighter us;
    [SerializeField] Fighter them;
    [SerializeField] TextMeshProUGUI tm;
    void Start()
    {
        tm.text = "Health:\nYou: " + us.HealthFormatted() + "\nthem: " + them.HealthFormatted();
    }

    // Update is called once per frame
    void Update()
    {
        tm.text = "Health:\nYou: " + us.HealthFormatted() + "\nthem: " + them.HealthFormatted();
    }
}
