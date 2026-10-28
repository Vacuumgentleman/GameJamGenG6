using GameJam;
using UnityEngine;
using UnityEngine.Events;

public class BoxMission : MonoBehaviour
{
    public BoxSlot[] slots; //arrastrt
    public UnityEvent alCompletarMision;

    bool misionCompleta = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (misionCompleta) return;

        //verificar si todos estan ocupados correctamente

        foreach (var slot in slots)
        {
            if (!slot.estaOcupadoCorrecto) return;
        }
        misionCompleta = true;
        Debug.Log("¡MISIÓN COMPLETADA!");
        alCompletarMision?.Invoke();
    }
}
