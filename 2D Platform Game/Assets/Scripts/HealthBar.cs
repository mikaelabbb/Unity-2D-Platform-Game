using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour
{
    private RectTransform bar;
    private Image barImage;
    private bool isDead;

    public GameManagerScript gameManager;

    // Start is called before the first frame update
    void Start()
    {
        bar = GetComponent<RectTransform>();
        barImage = GetComponent<Image>();
        if (Health.totalHealth < 0.3f)
        {
            barImage.color = Color.red;
        }
        SetSize(Health.totalHealth); //the health doesn't go back when reaching new lvl
    }

   public void Damage(float damage)
   {
        if ((Health.totalHealth -= damage) >= 0f)
        {
            Health.totalHealth -= damage;
        }
        else
        {
            Health.totalHealth = 0f;
        }
        
        if(Health.totalHealth <= 0f && !isDead)
        {
            isDead = true;
            Health.totalHealth = 0f;
            gameManager.gameOver();
        }

        SetSize(Health.totalHealth);

        if (Health.totalHealth < 0.3f) //changes the healthbar color to red when below 30% 
        {
            barImage.color = Color.red;
        }
    }

    public void SetSize(float size)
    {
        bar.localScale = new Vector3(size, 1f);
    }

}
