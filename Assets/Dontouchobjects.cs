using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Dontouchobjects : MonoBehaviour
{
    public GameObject PlayerDiedAwkwardlyPanel, HPCanvas;
    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HPCanvas.SetActive(false);
            PlayerDiedAwkwardlyPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
