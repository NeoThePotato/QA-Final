using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private GameObject shieldGameObject;
    
    [SerializeField] private int ActiveShieldChance; 
    [SerializeField] private float ActiveShieldTimeMin, ActiveShieldTimeMax;
    
    #endregion
    
    private void Start()
    {
        Invoke("ActivateShield", Random.Range(ActiveShieldTimeMin, ActiveShieldTimeMax));
    }

    //coroutine making a shot
    void ActivateShield() 
    {
        if (Random.value < (float)ActiveShieldChance / 100)                             //if random value less than shot probability, making a shot
        {                         
            shieldGameObject.SetActive(true);       
            Invoke("DeactiveShield", Random.Range(ActiveShieldTimeMin, ActiveShieldTimeMax));
        }
    }
    
    void DeactiveShield() 
    {
        shieldGameObject.SetActive(false); 
    }
}
