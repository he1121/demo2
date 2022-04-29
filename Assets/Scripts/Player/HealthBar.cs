using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public static int healthCurrent;
    public static int healthMax;

    public Image healthBar;
    void Start()
    {
        
    }

    void Update()
    {
        healthBar.fillAmount = (float)healthCurrent / healthMax;
    }
}
