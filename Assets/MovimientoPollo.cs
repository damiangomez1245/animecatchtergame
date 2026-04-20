using UnityEngine;
using System.Collections;

public class MovimientoPollo : MonoBehaviour
{
    public float velocidad = 2f;
    public float tiempoCaminando = 3f; // Segundos que dura caminando
    public float tiempoComiendo = 2f;  // Segundos que se para a comer

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool moviendoDerecha = true;
    private bool estaCaminando = true;

    void Start()
    {
        // Conectamos el script con los componentes del pollo
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Iniciamos la rutina de comportamiento
        StartCoroutine(RutinaDelPollo());
    }

    void Update()
    {
        // Solo lo movemos de lugar si está en estado "caminando"
        if (estaCaminando)
        {
            float direccion = moviendoDerecha ? 1f : -1f;
            transform.Translate(Vector2.right * direccion * velocidad * Time.deltaTime);
        }
    }

    // Esta es una función especial (Corrutina) que nos permite poner pausas de tiempo
    IEnumerator RutinaDelPollo()
    {
        while (true) // Bucle infinito para que lo haga siempre
        {
            // 1. ESTADO: CAMINAR
            estaCaminando = true;
            animator.SetBool("Caminando", true); // Le avisa al Animator que ponga la animación
            yield return new WaitForSeconds(tiempoCaminando); // Espera 3 segundos

            // 2. ESTADO: COMER
            estaCaminando = false;
            animator.SetBool("Caminando", false); // Quita la animación de caminar
            yield return new WaitForSeconds(tiempoComiendo); // Espera 2 segundos comiendo

            // 3. CAMBIAR DIRECCIÓN
            moviendoDerecha = !moviendoDerecha; // Invierte la dirección
            
            // Voltea el dibujo (ajusta a true/false si tu dibujo original mira a la izquierda)
            spriteRenderer.flipX = !moviendoDerecha; 
        }
    }
}