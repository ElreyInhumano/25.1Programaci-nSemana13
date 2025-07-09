using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NameUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField name_InputField;
    [SerializeField] private Button buttonName, buttonRed, buttonBlue, buttonYellow, buttonGreen;
    [SerializeField] private GameObject changeUIParametersPanel, panelColors;

    void Start()
    {
        Time.timeScale = 0;
        buttonName.onClick.AddListener(SetName);
        buttonRed.onClick.AddListener(SetColorRed);
        buttonBlue.onClick.AddListener(SetColorBlue);
        buttonYellow.onClick.AddListener(SetColorYellow);
        buttonGreen.onClick.AddListener(SetColorGreen);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            changeUIParametersPanel.SetActive(true);
            panelColors.SetActive(true);
        }
    }
    void SetName()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if (player.GetComponent<Player>().IsOwner)
            {
                player.GetComponent<Player>().SetName(name_InputField.text);
            }
        }
        panelColors.SetActive(false);
        changeUIParametersPanel.SetActive(false);
        Time.timeScale = 1;
    }
    void SetColorRed()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if (player.GetComponent<Player>().IsOwner)
            {
                player.GetComponent<Player>().SetColor("red");
            }
        }
        changeUIParametersPanel.SetActive(false);
        panelColors.SetActive(false);
        Time.timeScale = 1;
    }
    void SetColorBlue()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if (player.GetComponent<Player>().IsOwner)
            {
                player.GetComponent<Player>().SetColor("blue");
            }
        }
        changeUIParametersPanel.SetActive(false);
        panelColors.SetActive(false);
        Time.timeScale = 1;
    }
    void SetColorYellow()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if (player.GetComponent<Player>().IsOwner)
            {
                player.GetComponent<Player>().SetColor("yellow");
            }
        }
        changeUIParametersPanel.SetActive(false);
        panelColors.SetActive(false);
        Time.timeScale = 1;
    }
    void SetColorGreen()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if (player.GetComponent<Player>().IsOwner)
            {
                player.GetComponent<Player>().SetColor("green");
            }
        }
        changeUIParametersPanel.SetActive(false);
        panelColors.SetActive(false);
        Time.timeScale = 1;
    }
}
