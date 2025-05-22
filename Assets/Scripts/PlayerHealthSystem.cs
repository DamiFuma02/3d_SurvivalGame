using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSystem : MonoBehaviour
{
    public int maxHealth = 1000;
    public int currHealth = 1000;
    public bool isDead;
    public GameObject healthBarCanvas;
    private Slider healthBarSlider;
    private Image healthBarImg;
    private TextMeshProUGUI healthBarStatus;
    private Color32 highHealthColor = new Color32(23,132,18,255);
    private Color32 mediumHealthColor = new Color32(166,137,31,255);
    private Color32 lowHealthColor = new Color32(255,0,0,255);   


    // Start is called before the first frame update
    void Start()
    {
        healthBarSlider = healthBarCanvas.GetComponent<Slider>();
        healthBarSlider.value = 1f; // init full health
        healthBarImg = healthBarCanvas.transform.GetChild(1).gameObject.GetComponent<Image>();
        healthBarStatus = healthBarCanvas.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
        UpdateHealthBarUI();
        StartCoroutine(TakeDamage());
    }


    void UpdateHealthBarUI() {
        healthBarStatus.text = $"{currHealth}/{maxHealth}";
        if (currHealth >= maxHealth*75/100) {
            healthBarImg.color = highHealthColor;
        }
        else if (currHealth >= maxHealth*35/100) {
            healthBarImg.color = mediumHealthColor;
        }
        else {
            healthBarImg.color = lowHealthColor;
        }
    }




    private IEnumerator TakeDamage() {
        currHealth -= 100;
        healthBarSlider.value = (float)currHealth / maxHealth;
        UpdateHealthBarUI();
        if (currHealth <= 0) {
            StopAllCoroutines();
            yield return null;
        } else {
            yield return new WaitForSeconds(5f);
            StartCoroutine(TakeDamage());
        }
        
    }
}
