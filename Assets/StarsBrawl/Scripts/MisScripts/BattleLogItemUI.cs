using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class BattleLogItemUI : MonoBehaviour
{
    [Header("Componentes Visuales")]
    public TextMeshProUGUI modeText;     
    public TextMeshProUGUI modalityText; 
    public TextMeshProUGUI resultText;   
    public TextMeshProUGUI dateText;    

    
    private string currentMatchId;

    
    public void Setup(string matchId, string mode, string modality, string result, string date)
    {
        currentMatchId = matchId;

        if (modeText != null) modeText.text = mode;
        if (modalityText != null) modalityText.text = modality;
        if (resultText != null) resultText.text = result;
        if (dateText != null) dateText.text = date;
    }

   
    public void OnDetailsButtonClicked()
    {
      
        UserSession.SelectedMatchID = currentMatchId;

        
        SceneManager.LoadScene("MatchDetails");
    }
}