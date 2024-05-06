using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float RegenSpeed = 2f;
    public float HealthLerpSpeed = 5f;
    public GameObject UI_Left;
    public GameObject UI_Right;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //850 is normal
        RectTransform rectTransformLeft = UI_Left.GetComponent<RectTransform>();
        rectTransformLeft.anchoredPosition = Vector2.Lerp(rectTransformLeft.anchoredPosition, new Vector2(rectTransformLeft.anchoredPosition.x, -8.5f * Health), HealthLerpSpeed * Time.deltaTime);

        RectTransform rectTransformRight = UI_Right.GetComponent<RectTransform>();
        rectTransformRight.anchoredPosition = Vector2.Lerp(rectTransformRight.anchoredPosition3D, new Vector2(rectTransformRight.anchoredPosition.x, 8.5f * Health), HealthLerpSpeed * Time.deltaTime);
        if (Health < MaxHealth)
        {
            Health += RegenSpeed * Time.deltaTime;
        } else
        {
            Health = MaxHealth;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DestroyableObject"))
        {
            Health = Health - 1f;
            //Debug.Log("Health: " + Health);

            if (Health <= 0f)
            {
                Application.Quit();
            }
        } 
    }
}
