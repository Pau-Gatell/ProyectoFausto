using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenuUI;           // Canvas del menú principal (escena aparte)

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;          // Canvas del menú de pausa

    private bool isPaused = false;

    void Start()
    {
        // Aseguramos que el menú de pausa empiece desactivado
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Si estamos en la escena del menú principal, ocultamos el cursor
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        // Solo permitimos pausar en la escena del juego
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                    Resume();
                else
                    Pause();
            }
        }
    }

    // ====================== MENÚ PRINCIPAL ======================

    public void OpenNorthpoint()
    {
        Application.OpenURL("https://northpointwear.com/");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");   // Cambia si tu escena se llama diferente
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }

    // ====================== MENÚ DE PAUSA ======================

    public void Pause()
    {
        if (pauseMenuUI == null) return;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        if (pauseMenuUI == null) return;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");     // Nombre exacto de tu escena del menú
    }
}