using UnityEngine;

public class ClothingShop2 : MonoBehaviour
{
    [Header("URL de compra")]
    public string shopURL = "https://northpointwear.com/tienda/all/t-shirt-bior-works-logo/";

    private bool playerInside = false;

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            Application.OpenURL(shopURL);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}