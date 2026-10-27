using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Bonfire : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI counterText; 

    public float timeToBurn = 5f; //tiempo entre cada ciclo
    public int requiredAmount = 3; // burneables
    public GameObject objectToDestroy;

    private List<GameObject> objectsOnFire = new List<GameObject>(); // Lista del store de todos los objetos quemables que estan actualmente dentro del fuego
    private int collectedCount = 0;
     void Start()
     {
        InvokeRepeating(nameof(BurnObjects), timeToBurn, timeToBurn);
        if(counterText != null)
            counterText.text = $"Quemados: {collectedCount} / {requiredAmount}";
     }
    
    //cuando un objeto entra a la hoguera
     void OnTriggerEnter(Collider other)

     { // Evita añadir el mismo objeto dos veces y comprueba que el objeto tenga la etiqueta correcta
        if (other.CompareTag("Burneable") && !objectsOnFire.Contains(other.gameObject))
        {
          
            objectsOnFire.Add(other.gameObject);
           // Debug.Log(other.name + "entró a la hoguera");
        }
     }
    //Cuando un objeto sale de la hoguera
    private void OnTriggerExit(Collider other)
    {
        if (objectsOnFire.Contains(other.gameObject))
        {
            objectsOnFire.Remove(other.gameObject);
        }
    }
   

    // Update is called once per frame
    void Update()
    {
        
    }
    // quemar los objetos
    void BurnObjects()
    {
        if (objectsOnFire.Count == 0) return;
        //crea una lista para evitar modificarla durante el ciclo foreach
        foreach (var obj in new List<GameObject>(objectsOnFire))
            if (obj != null)
            {
                objectsOnFire.Remove(obj);
                Destroy(obj);

                // Debug.Log(obj.name + "was burned");
                collectedCount++; //suma por cada objeto quemado
                Debug.Log($"Quemados: {collectedCount} / {requiredAmount}");

                // actualiza la UI
                if (counterText != null)
                {
                    counterText.text = $"Quemados: {collectedCount} / {requiredAmount}";
                }
            }
        if (BurnCounter.Instance != null)
        {
            BurnCounter.Instance.AddBurned();
        }
    }
    void CheckRequiriment()
    {
        if (collectedCount >= requiredAmount)
        {
            counterText.text = $"COMPLETADO: {collectedCount} / {requiredAmount}";

            Debug.Log("¡Requisito cumplido!");
            if (objectToDestroy != null)
            {
                Destroy(objectToDestroy);
            }
            else
            {
                Destroy(gameObject); 
            }

        }


    }
    ///CheckRequiriment();
}
