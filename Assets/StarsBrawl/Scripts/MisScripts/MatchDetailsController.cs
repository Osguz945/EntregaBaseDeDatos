using UnityEngine;
using TMPro;
using MySqlConnector;

public class MatchDetailsController : MonoBehaviour
{
    [Header("Resultado General de la Partida")]
    public TextMeshProUGUI matchResultText; 

    [Header("Equipos (Contenedores)")]
    public Transform winnersContainer;     
    public Transform losersContainer;      
    public GameObject participantPrefab;   

    private string connectionString = "Server=127.0.0.1; Database=brawl_stars; User ID=root; Password=root; SslMode=None;";

    void Start()
    {
        if (string.IsNullOrEmpty(UserSession.SelectedMatchID))
        {
            Debug.LogError(" ERROR: El ID de la partida está vacío.");
            return;
        }

        LoadMatchDetails(UserSession.SelectedMatchID, UserSession.CurrentUserID);
    }

    void LoadMatchDetails(string matchId, string currentUserId)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            
            string queryUserResult = "SELECT result FROM MATCH_USER WHERE match_id = @matchId AND user_id = @userId";
            using (var cmd = new MySqlCommand(queryUserResult, connection))
            {
                cmd.Parameters.AddWithValue("@matchId", matchId);
                cmd.Parameters.AddWithValue("@userId", currentUserId);

                var res = cmd.ExecuteScalar();
                if (res != null && matchResultText != null)
                {
                    string finalResult = res.ToString();
                    matchResultText.text = finalResult.ToUpper();

                    
                    if (finalResult.ToLower().Contains("vict") || finalResult.ToLower().Contains("win"))
                        matchResultText.color = Color.green;
                    else
                        matchResultText.color = Color.red;
                }
            }

            
            string queryParticipants = @"
                SELECT u.nickname, mu.brawler_id, mu.result, 
                       IFNULL(ub.trophies, 0) AS trophies,
                       IFNULL(ub.level, 1) AS brawler_level
                FROM MATCH_USER mu
                JOIN USERS u ON mu.user_id = u.user_id
                LEFT JOIN USER_BRAWLERS ub ON mu.user_id = ub.user_id AND mu.brawler_id = ub.brawler_id
                WHERE mu.match_id = @matchId";

            using (var cmd = new MySqlCommand(queryParticipants, connection))
            {
                cmd.Parameters.AddWithValue("@matchId", matchId);
                using (var reader = cmd.ExecuteReader())
                {
                    PortraitProvider portraitProvider = FindObjectOfType<PortraitProvider>();

                    while (reader.Read())
                    {
                        string nickname = reader["nickname"].ToString();
                        int brawlerId = reader.GetInt32("brawler_id");
                        string pResult = reader["result"].ToString().ToLower();
                        int trophies = reader.GetInt32("trophies");
                        int brawlerLevel = reader.GetInt32("brawler_level"); 

                        Transform targetContainer = pResult.Contains("vict") || pResult.Contains("win") ? winnersContainer : losersContainer;

                        if (targetContainer != null && participantPrefab != null)
                        {
                            GameObject playerObj = Instantiate(participantPrefab, targetContainer);
                            MatchParticipantItemUI itemUI = playerObj.GetComponent<MatchParticipantItemUI>();

                            if (itemUI != null)
                            {
                                Sprite brawlerSprite = null;
                                if (portraitProvider != null)
                                {
                                    portraitProvider.TryGetPortraitByID(brawlerId, out brawlerSprite);
                                }

                                
                                itemUI.Setup(nickname, brawlerSprite, trophies, brawlerLevel);
                            }
                        }
                    }
                }
            }
        }
    }
}