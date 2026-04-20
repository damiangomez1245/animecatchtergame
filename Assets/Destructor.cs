using UnityEngine;

public class Destructor : MonoBehaviour
{
    // Esta función se activa cuando algo choca contra este objeto invisible
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Destruye el objeto que lo tocó (la manzana, la bomba, etc.)
        Destroy(collision.gameObject);
    }
}