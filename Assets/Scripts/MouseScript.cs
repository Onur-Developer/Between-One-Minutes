using UnityEngine;

public class MouseScript : MonoBehaviour
{
    [SerializeField] private GameObject particleObj;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        particleObj.SetActive(true);
    }

    private void OnEnable()
    {
        GameManager.instance.jn.mainPlayer = GetComponent<MainPlayer>();
    }

    private void OnDisable()
    {
        particleObj.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Play"))
        {
            GameManager.instance.RestartButton();
            GameManager.instance.mainMenu.SetActive(false);
            transform.position = Vector2.zero;
            GameManager.instance.gameManagerUI.SetActive(true);
            gameObject.SetActive(false);
            settingsPanel.SetActive(false);
        }
        else if (col.CompareTag("Replay"))
        {
            GameManager.instance.RestartButton();
            transform.position = Vector2.zero;
            Transform ourParent = col.transform.parent;
            for (int i = 0; i < ourParent.childCount; i++)
            {
                Destroy(ourParent.GetChild(i).gameObject);
            }
            GameManager.instance.gameManagerUI.SetActive(true);
            gameObject.SetActive(false);
        }
        else if (col.CompareTag("Audio"))
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
        
        else if (col.CompareTag("Home"))
        {
            particleObj.SetActive(true);
            Transform ourParent = col.transform.parent;
            for (int i = 0; i < ourParent.childCount; i++)
            {
                Destroy(ourParent.GetChild(i).gameObject);
            }
            GameManager.instance.mainMenu.SetActive(true);
            GameManager.instance.gameManagerUI.SetActive(false);
        }
        else if (col.CompareTag("Quit"))
        {
            Application.Quit();
        }
        GameManager.instance.scorePanel.SetActive(false);
    }
}
