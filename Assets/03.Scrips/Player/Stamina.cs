using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    // Player Specification
    public float stamina = 1000f;
    private float maxStamina;

    // References
    public Text stText;
    public RectTransform stBar;
    private PlayerCtrl playerctrl;

    // Use this for initialization
    private void Start()
    {
        playerctrl = GetComponent<PlayerCtrl>();

        maxStamina = stamina;
        stText.text = ((int)(stamina / maxStamina * 100f)).ToString() + "%";
        stBar.localScale = Vector3.one;
    }

    private void Update()
    {
        if (/*playerctrl.velocity.sqrMagnitude > 99 &&*/ Input.GetKey(KeyCode.LeftShift) && stamina > 0)
        {
            stamina--;
            UpdateST();
        }
        else if (stamina < maxStamina)
        {
            stamina += .5f;
            UpdateST();
        }
    }

    private void UpdateST()
    {
        stText.text = ((int)(stamina / maxStamina * 100f)).ToString() + "%";
        stBar.localScale = new Vector3(stamina / maxStamina, 1, 1);
    }
}
