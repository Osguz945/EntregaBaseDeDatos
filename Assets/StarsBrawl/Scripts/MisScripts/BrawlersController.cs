using UnityEngine;
using TMPro;
using MySqlConnector;
using UnityEngine.UI;

public class BrawlersController : MonoBehaviour
{
    [Header("Interfaz General")]
  
    public TextMeshProUGUI brawlersCountText;

    [Header("Lista de Brawlers")]
    public Transform brawlersContainer; 
    public GameObject brawlerPrefab; 

    private string connectionString = "Server=127.0.0.1; Database=brawl_stars; User ID=root; Password=root; SslMode=None;";

    void Start()
    {
        
        if (string.IsNullOrEmpty(UserSession.CurrentUserID))
        {
            Debug.LogError("No hay un usuario logueado en la sesión.");
            return;
        }

        LoadBrawlersData(UserSession.CurrentUserID);
    }

    void LoadBrawlersData(string userId)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

          
            int totalBrawlers = 0;
            int unlockedBrawlers = 0;

            string queryTotal = "SELECT COUNT(*) FROM BRAWLERS";
            using (var cmd = new MySqlCommand(queryTotal, connection))
            {
                totalBrawlers = System.Convert.ToInt32(cmd.ExecuteScalar());
            }

            string queryUnlocked = "SELECT COUNT(*) FROM USER_BRAWLERS WHERE user_id = @id";
            using (var cmd = new MySqlCommand(queryUnlocked, connection))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                unlockedBrawlers = System.Convert.ToInt32(cmd.ExecuteScalar());
            }

            if (brawlersCountText != null)
                brawlersCountText.text = $"{unlockedBrawlers}/{totalBrawlers}";

           
            string queryList = @"
                SELECT b.brawler_id, b.name, ub.level, ub.trophies 
                FROM USER_BRAWLERS ub 
                JOIN BRAWLERS b ON ub.brawler_id = b.brawler_id 
                WHERE ub.user_id = @id";

            using (var cmd = new MySqlCommand(queryList, connection))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    
                    PortraitProvider portraitProvider = FindObjectOfType<PortraitProvider>();

                    while (reader.Read())
                    {
                       
                        int brawlerId = reader.GetInt32("brawler_id");
                        string brawlerName = reader.GetString("name");
                        int level = reader.GetInt32("level");
                        int trophies = reader.GetInt32("trophies");

                        
                        GameObject brawlerObj = Instantiate(brawlerPrefab, brawlersContainer);
                        BrawlerItemUI itemUI = brawlerObj.GetComponent<BrawlerItemUI>();

                        if (itemUI != null)
                        {
                            Sprite portrait = null;

                            
                            if (portraitProvider != null)
                            {
                                
                                portraitProvider.TryGetPortraitByID(brawlerId, out portrait);
                            }

                            
                            itemUI.Setup(brawlerName, level, trophies, portrait, brawlerId.ToString());
                        }
                    }
                }
            }
        }
    }
}