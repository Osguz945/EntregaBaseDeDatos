using UnityEngine;
using MySqlConnector;

public class BattleLogController : MonoBehaviour
{
    [Header("Lista de Partidas")]
    public Transform battleLogContainer; 
    public GameObject battleLogPrefab;   

    private string connectionString = "Server=127.0.0.1; Database=brawl_stars; User ID=root; Password=root; SslMode=None;";

    void Start()
    {
        if (string.IsNullOrEmpty(UserSession.CurrentUserID))
        {
            Debug.LogError("No hay un usuario logueado en la sesión.");
            return;
        }

        LoadBattleLog(UserSession.CurrentUserID);
    }

    void LoadBattleLog(string userId)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

           
            
            string query = @"
                SELECT m.match_id, m.date, mt.name AS mode_name, mt.modality, mu.result 
                FROM MATCH_USER mu
                JOIN MATCHES m ON mu.match_id = m.match_id
                JOIN MATCH_TYPES mt ON m.match_type_id = mt.match_type_id
                WHERE mu.user_id = @id
                ORDER BY m.date DESC";

            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@id", userId);

                using (var reader = cmd.ExecuteReader())
                {
                   

                    while (reader.Read())
                    {
                        
                        string matchId = reader["match_id"].ToString();
                        string modeName = reader["mode_name"].ToString();
                        string modality = reader["modality"].ToString(); 
                        string result = reader["result"].ToString();

                       
                        string date = reader.GetDateTime("date").ToString("dd/MM/yyyy");

                        
                        GameObject matchObj = Instantiate(battleLogPrefab, battleLogContainer);
                        BattleLogItemUI itemUI = matchObj.GetComponent<BattleLogItemUI>();

                        if (itemUI != null)
                        {
                            
                            itemUI.Setup(matchId, modeName, modality, result, date);
                        }
                    }
                }
            }
        }
    }
}