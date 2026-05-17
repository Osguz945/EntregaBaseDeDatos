using UnityEngine;
using TMPro;
using MySqlConnector;

public class BrawlerDetailsController : MonoBehaviour
{
    [Header("Datos Generales del Brawler")]
    public TextMeshProUGUI brawlerNameText;
    public TextMeshProUGUI classNameText;
    public TextMeshProUGUI rarityNameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackNameText;      
    public TextMeshProUGUI superNameText;        
    public TextMeshProUGUI traitDescriptionText; 
    public TextMeshProUGUI descriptionText;

    [Header("Datos del Jugador con el Brawler")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI trophiesText;

    private string connectionString = "Server=127.0.0.1; Database=brawl_stars; User ID=root; Password=root; SslMode=None;";

    void Start()
    {
        if (string.IsNullOrEmpty(UserSession.SelectedBrawlerID))
        {
            Debug.LogError("ERROR: No se ha seleccionado ningún Brawler en la sesión.");
            return;
        }

        LoadBrawlerDetails(UserSession.SelectedBrawlerID, UserSession.CurrentUserID);
    }

    void LoadBrawlerDetails(string brawlerId, string userId)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

           
            string query = @"
                SELECT b.name AS brawler_name, b.description, b.health,
                       c.name AS class_name, r.name AS rarity_name,
                       a.name AS attack_name, s.name AS super_name, t.description AS trait_description,
                       ub.level, ub.trophies
                FROM BRAWLERS b
                JOIN CLASSES c ON b.class_id = c.class_id
                JOIN RARITIES r ON b.rarity_id = r.rarity_id
                JOIN ATTACKS a ON b.attack_id = a.attack_id
                JOIN SUPERS s ON b.super_id = s.super_id
                LEFT JOIN TRAITS t ON b.trait_id = t.trait_id
                JOIN USER_BRAWLERS ub ON b.brawler_id = ub.brawler_id
                WHERE b.brawler_id = @brawlerId AND ub.user_id = @userId";

            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@brawlerId", brawlerId);
                cmd.Parameters.AddWithValue("@userId", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        
                        if (brawlerNameText != null) brawlerNameText.text = reader["brawler_name"].ToString();
                        if (classNameText != null) classNameText.text = reader["class_name"].ToString();
                        if (rarityNameText != null) rarityNameText.text = reader["rarity_name"].ToString();
                        if (healthText != null) healthText.text = reader["health"].ToString();
                        if (descriptionText != null) descriptionText.text = reader["description"].ToString();

                       
                        if (attackNameText != null) attackNameText.text = reader["attack_name"].ToString();
                        if (superNameText != null) superNameText.text = reader["super_name"].ToString();

                        
                        if (traitDescriptionText != null)
                        {
                            var traitValue = reader["trait_description"];
                            if (traitValue != System.DBNull.Value && !string.IsNullOrEmpty(traitValue.ToString()))
                            {
                                traitDescriptionText.text = traitValue.ToString();
                            }
                            else
                            {
                                
                                traitDescriptionText.text = "Este brawler no posee un atributo especial.";
                            }
                        }

                        if (levelText != null) levelText.text = reader["level"].ToString();
                        if (trophiesText != null) trophiesText.text = reader["trophies"].ToString();
                    }
                    else
                    {
                        Debug.LogWarning(" No se encontró la información del Brawler en la Base de Datos para este usuario.");
                    }
                }
            }
        }
    }
}