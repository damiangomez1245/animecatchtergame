using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    [Header("Configuración del Juego")]
    public float tiempoRestante = 88f; // 1:28
    public int puntos = 0;
    public int metaPuntos = 50; // Meta a superar
    public bool juegoActivo = false;
    
    [Header("API Web")]
    // ¡ACTUALIZADO! URL local para entregar la tarea
    public string apiUrl = "http://localhost:3000/users"; 
    private string userId = "1";

    [Header("Interfaz (UI)")]
    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoPuntos;
    public GameObject pantallaInicio;
    public GameObject pantallaWin;
    public GameObject pantallaLose;
    public GameObject botonRetry; 

    [Header("Audios (Arrastra los objetos y clips)")]
    public AudioSource musicaFondo; // La bocina de música
    public AudioSource sfxEfectos;  // La bocina de efectos
    public AudioClip audioReadyGo;
    public AudioClip audioRisaWin;
    public AudioClip audioLlantoLose;

    private void Awake()
    {
        Instancia = this;
    }

    void Start()
    {
        // 1. Obtener el ID de la URL web (Requisito de la práctica)
        string urlCompleta = Application.absoluteURL;
        if (urlCompleta.Contains("?id=")) {
            userId = urlCompleta.Split(new string[] { "?id=" }, System.StringSplitOptions.None)[1];
        }

        // ¡NUEVO! Llamamos a la API para traer los datos del jugador al iniciar
        StartCoroutine(CargarDatosDelUsuario());

        // 2. Apagar las pantallas de fin y el botón al iniciar
        botonRetry.SetActive(false);
        pantallaWin.SetActive(false);
        pantallaLose.SetActive(false);

        // 3. Comenzar la secuencia de "Ready, Go"
        StartCoroutine(SecuenciaDeInicio());
    }

    IEnumerator SecuenciaDeInicio()
    {
        pantallaInicio.SetActive(true);
        
        // Reproducir el audio
        if (audioReadyGo != null) sfxEfectos.PlayOneShot(audioReadyGo);
        
        // Esperar 3 segundos (cámbialo si tu audio es más corto)
        yield return new WaitForSeconds(3f); 
        
        // Iniciar el nivel
        pantallaInicio.SetActive(false);
        juegoActivo = true;
        musicaFondo.Play(); 
    }

    void Update()
    {
        if (juegoActivo)
        {
            tiempoRestante -= Time.deltaTime;

            // El juego termina SOLO cuando el reloj llega a 0
            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0;
                TerminarJuego(); 
            }

            ActualizarTextos();
        }
    }

    // Se llama desde el jugador cuando atrapa cosas
    public void SumarPuntos(int cantidad)
    {
        if (!juegoActivo) return;

        puntos += cantidad;
        if (puntos < 0) puntos = 0; // Evitar manzanas negativas
        
        ActualizarTextos();
    }

    void ActualizarTextos()
    {
        int minutos = Mathf.FloorToInt(tiempoRestante / 60);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60);
        textoTiempo.text = string.Format("{0:0}:{1:00}", minutos, segundos);
        textoPuntos.text = "Manzanas: " + puntos;
    }

    void TerminarJuego()
    {
        juegoActivo = false;
        musicaFondo.Stop(); 

        // Checar si superó la meta de 50 puntos
        if (puntos >= metaPuntos)
        {
            pantallaWin.SetActive(true);
            if (audioRisaWin != null) sfxEfectos.PlayOneShot(audioRisaWin);
        }
        else
        {
            pantallaLose.SetActive(true);
            if (audioLlantoLose != null) sfxEfectos.PlayOneShot(audioLlantoLose);
        }

        // Enviar a la base de datos
        StartCoroutine(GuardarPuntajeEnAPI());

        // Retrasar la aparición del botón 1 segundo
        StartCoroutine(MostrarBotonConRetraso());
    }

    IEnumerator MostrarBotonConRetraso()
    {
        yield return new WaitForSeconds(1f);
        botonRetry.SetActive(true);
    }

    public void ReintentarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator GuardarPuntajeEnAPI()
    {
        string json = "{\"score\": " + puntos + "}";
        using (UnityWebRequest webRequest = UnityWebRequest.Put(apiUrl + "/" + userId + "/score", json))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
        }
    }

    // ¡NUEVO! Función para cargar los datos al inicio y cumplir la rúbrica
    IEnumerator CargarDatosDelUsuario()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl + "/" + userId))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Datos del jugador cargados: " + webRequest.downloadHandler.text);
            }
            else
            {
                Debug.Log("Error al cargar datos: " + webRequest.error);
            }
        }
    }
}