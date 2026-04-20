using UnityEngine;
using System.Collections;

public class GeneradorObjetos : MonoBehaviour
{
    [Header("Los Prefabs (Arrastra los cuadritos azules aquí)")]
    public GameObject prefabManzana;
    public GameObject prefabMisil;
    public GameObject prefabBomba;
    
    [Header("Configuración")]
    public float tiempoSpawn = 0.2f; // Caen cada 0.2 segundos
    public float rangoAncho = 8f; // Qué tan ancho es tu nivel

    void Start()
    {
        // Inicia la lluvia infinita
        StartCoroutine(RutinaLluvia());
    }

    IEnumerator RutinaLluvia()
    {
        while (true) // Repetir por siempre
        {
            // Esperar el tiempo indicado
            yield return new WaitForSeconds(tiempoSpawn);

            // ¡AQUÍ ESTÁ EL ARREGLO! Solo generamos objetos si el juego sigue activo
            if (GameManager.Instancia != null && GameManager.Instancia.juegoActivo == true)
            {
                // Elegir una posición X aleatoria
                float posicionXAleatoria = Random.Range(-rangoAncho, rangoAncho);
                Vector3 puntoDeCaida = new Vector3(posicionXAleatoria, transform.position.y, 0f);

                // Elegir qué objeto va a caer
                int probabilidad = Random.Range(0, 100);

                if (probabilidad < 75) 
                {
                    Instantiate(prefabManzana, puntoDeCaida, Quaternion.identity);
                }
                else if (probabilidad < 90)
                {
                    Instantiate(prefabMisil, puntoDeCaida, Quaternion.identity);
                }
                else
                {
                    Instantiate(prefabBomba, puntoDeCaida, Quaternion.identity);
                }
            }
        }
    }
}
