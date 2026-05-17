using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class BrawlerItemUI : MonoBehaviour
{
    [Header("Componentes Visuales")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI trophiesText;
    public Image portraitImage;

    private string brawlerId;

    public void Setup(string name, int level, int trophies, Sprite portrait, string id)
    {
        brawlerId = id;

        if (nameText != null) nameText.text = name;
        if (levelText != null) levelText.text = "Nivel " + level;
        if (trophiesText != null) trophiesText.text = trophies.ToString();

        if (portraitImage != null && portrait != null)
        {
            portraitImage.sprite = portrait;
        }
    }

   
    public void OnBrawlerClicked()
    {
       
        UserSession.SelectedBrawlerID = brawlerId;

        
        SceneManager.LoadScene("BrawlerDetails");
    }
}