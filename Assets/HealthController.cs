using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{

    public float maxHealth;

    public float currentHealth;

    public Slider MaxSlider;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    
    public void TakeDamage()
    {
        currentHealth -= MaxSlider.value;
        Debug.Log(currentHealth);
        
        if (currentHealth <= 0)
        {
            Debug.Log("Try: Destroy Wood");
        }
        
    }

    
    
}
