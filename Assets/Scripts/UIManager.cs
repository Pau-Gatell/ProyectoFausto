using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void OpenNorthpoint()
    {
        Application.OpenURL("https://northpointwear.com/"); // cambia por la web real
    }


    // ▶️ Cargar escena del juego
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); // nombre EXACTO de tu escena
    }

    // ❌ Salir del juego
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego"); // para probar en Unity
    }
}
