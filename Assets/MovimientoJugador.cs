using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 8f; // Qué tan rápido corre
    public float limitePantalla = 8f; // Hasta dónde puede llegar a los lados

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator; // Referencia al Animator

    void Start()
    {
        // Conectamos el script con el cuerpo, el dibujo y el animador de la monita
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Obtenemos el Animator
    }

    void Update()
    {
        // 1. LEER EL TECLADO (Devuelve un número entre -1 y 1)
        float movimientoX = Input.GetAxis("Horizontal"); 

        // 2. MOVER A LA MONITA (Mantenemos la velocidad Y igual para que la gravedad funcione)
        rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);

        // 3. ACTUALIZAR LA ANIMACIÓN
        // Pasamos el valor absoluto del movimiento horizontal al parámetro "VelocidadX" del Animator
        // Usamos Mathf.Abs para que siempre sea positivo, ya sea que vaya a la izquierda (-1) o derecha (1)
        animator.SetFloat("VelocidadX", Mathf.Abs(movimientoX));

        // 4. VOLTEAR EL DIBUJO HACIA DONDE CAMINA
        if (movimientoX > 0.1f)
        {
            spriteRenderer.flipX = false; // Mira a la derecha
        }
        else if (movimientoX < -0.1f)
        {
            spriteRenderer.flipX = true; // Mira a la izquierda
        }

        // 5. EVITAR QUE SE SALGA DE LA PANTALLA
        float posXLimitada = Mathf.Clamp(transform.position.x, -limitePantalla, limitePantalla);
        transform.position = new Vector3(posXLimitada, transform.position.y, transform.position.z);
    }
    // Esta función detecta cuando la monita toca un objeto "Trigger" (como tus manzanas)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Si lo que tocó tiene la etiqueta "Manzana"
        if (collision.CompareTag("Manzana"))
        {
            Debug.Log("¡Atrapé una Manzana!"); // Manda un mensaje a la consola de Unity
            Destroy(collision.gameObject);     // Desaparece la manzana de la pantalla
            
             GameManager.Instancia.SumarPuntos(1); // (Esto lo activaremos en la Fase 4)
        }
        
        // 2. Si lo que tocó tiene la etiqueta "Misil"
        else if (collision.CompareTag("Misil"))
        {
            Debug.Log("¡Ouch! Un misil.");
            Destroy(collision.gameObject);     // Desaparece el misil
            
             GameManager.Instancia.SumarPuntos(-1); // (Esto lo activaremos en la Fase 4)
        }

        // 3. Si lo que tocó tiene la etiqueta "Bomba"
        else if (collision.CompareTag("Bomba"))
        {
            Debug.Log("¡Toqué una bomba!");
            Destroy(collision.gameObject);     // Desaparece la bomba
        }
    }
}