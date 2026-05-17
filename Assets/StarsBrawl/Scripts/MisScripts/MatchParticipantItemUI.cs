using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class MatchParticipantItemUI : MonoBehaviour
{
    [Header("Componentes Visuales de la Tarjeta de Jugador")]
    public TextMeshProUGUI nicknameText;     
    public Image brawlerPortraitImage;       
    public TextMeshProUGUI trophiesText;     
    public TextMeshProUGUI levelText;        

    
    public void Setup(string nickname, Sprite brawlerPortrait, int trophies, int level)
    {
        if (nicknameText != null)
            nicknameText.text = nickname;

        if (trophiesText != null)
            
            trophiesText.text = trophies.ToString() + " ";

        if (levelText != null)
           
            levelText.text = "" + level;

        if (brawlerPortraitImage != null && brawlerPortrait != null)
        {
            brawlerPortraitImage.sprite = brawlerPortrait;
        }
        else if (brawlerPortraitImage != null)
        {
            
            brawlerPortraitImage.gameObject.SetActive(false);
        }
    }
}