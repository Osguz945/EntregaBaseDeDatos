using UnityEngine;
using TMPro;
using MySqlConnector;

public class MainMenuController : MonoBehaviour
{
    [Header("General & Main Menu")]
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI totalTrophiesText;
    public TextMeshProUGUI coinsText, gemsText, blingText;

    [Header("UserDetails - Identificación")]
    public TextMeshProUGUI userIdText;
    public TextMeshProUGUI rankNameText;
    public UnityEngine.UI.Image rankIconImage;

    [Header("UserDetails - Estadísticas")]
    public TextMeshProUGUI wins3vs3Text;
    public TextMeshProUGUI winsSurvivalText;
    public TextMeshProUGUI currentStreakText;
    public TextMeshProUGUI favoriteBrawlerText;
    public TextMeshProUGUI winningestBrawlerText;
    public TextMeshProUGUI favoriteModeText;

    [Header("Último Brawler Usado")]
    public TextMeshProUGUI lastBrawlerLevelText;
    public TextMeshProUGUI lastBrawlerTrophiesText;

    private string connectionString = "Server=127.0.0.1; Database=brawl_stars; User ID=root; Password=root; SslMode=None;";

    void Start()
    {
        if (string.IsNullOrEmpty(UserSession.CurrentUserID)) return;

        LoadAllPlayerData();
    }

    void LoadAllPlayerData()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
            }
            catch (System.Exception e)
            {
                Debug.LogError(" No se pudo abrir la conexión a la Base de Datos: " + e.Message);
                return;
            }

            string id = UserSession.CurrentUserID;
            if (userIdText != null) userIdText.text = id;

          
            try
            {
                string sqlUser = "SELECT nickname, coins, gems, bling FROM USERS WHERE user_id = @id";
                using (var cmdUser = new MySqlCommand(sqlUser, connection))
                {
                    cmdUser.Parameters.AddWithValue("@id", id);
                    using (var reader = cmdUser.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (nicknameText != null) nicknameText.text = reader.GetString("nickname");
                            if (coinsText != null) coinsText.text = reader.GetInt32("coins").ToString();
                            if (gemsText != null) gemsText.text = reader.GetInt32("gems").ToString();
                            if (blingText != null) blingText.text = reader.GetInt32("bling").ToString();
                        }
                    }
                }
            }
            catch (System.Exception e) { Debug.LogError(" Error al cargar Datos Básicos: " + e.Message); }


          
            int trophies = 0;
            try
            {
                string sqlTrophies = "SELECT SUM(trophies) FROM USER_BRAWLERS WHERE user_id = @id";
                using (var cmdTrophies = new MySqlCommand(sqlTrophies, connection))
                {
                    cmdTrophies.Parameters.AddWithValue("@id", id);
                    var totalTrophies = cmdTrophies.ExecuteScalar();
                    if (totalTrophies != System.DBNull.Value && totalTrophies != null)
                    {
                        trophies = System.Convert.ToInt32(totalTrophies);
                    }
                }

                if (totalTrophiesText != null) totalTrophiesText.text = trophies.ToString();

                if (trophies > 0)
                {
                    string sqlRank = "SELECT rank_id, name FROM RANKS WHERE min_trophies <= @trophies AND max_trophies >= @trophies";
                    using (var cmdRank = new MySqlCommand(sqlRank, connection))
                    {
                        cmdRank.Parameters.AddWithValue("@trophies", trophies);
                        using (var readerRank = cmdRank.ExecuteReader())
                        {
                            if (readerRank.Read())
                            {
                                int rankId = readerRank.GetInt32("rank_id");
                                string rankName = readerRank.GetString("name");

                                if (rankNameText != null) rankNameText.text = rankName;

                                RankIconProvider rankIconProvider = FindObjectOfType<RankIconProvider>();
                                if (rankIconProvider != null && rankIconImage != null)
                                {
                                    if (rankIconProvider.TryGetRankIconByID(rankId, out Sprite rankSprite))
                                    {
                                        rankIconImage.sprite = rankSprite;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (rankNameText != null) rankNameText.text = "Sin Rango";
                }
            }
            catch (System.Exception e) { Debug.LogError(" Error al cargar Trofeos/Rango: " + e.Message); }


        
            try
            {
                string sql3v3 = @"
                    SELECT COUNT(*) FROM MATCH_USER mu 
                    JOIN MATCHES m ON mu.match_id = m.match_id 
                    JOIN MATCH_TYPES mt ON m.match_type_id = mt.match_type_id 
                    WHERE mu.user_id = @id 
                      AND (mu.result LIKE '%vict%' OR mu.result LIKE '%win%') 
                      AND mt.modality = '3vs3'";

                using (var cmd3v3 = new MySqlCommand(sql3v3, connection))
                {
                    cmd3v3.Parameters.AddWithValue("@id", id);
                    if (wins3vs3Text != null) wins3vs3Text.text = cmd3v3.ExecuteScalar().ToString();
                }
            }
            catch (System.Exception e) { Debug.LogError(" Error en query de Victorias 3vs3: " + e.Message); }


        
            try
            {
                string sqlSurv = @"
                    SELECT COUNT(*) FROM MATCH_USER mu 
                    JOIN MATCHES m ON mu.match_id = m.match_id 
                    JOIN MATCH_TYPES mt ON m.match_type_id = mt.match_type_id 
                    WHERE mu.user_id = @id 
                      AND (mu.result LIKE '%vict%' OR mu.result LIKE '%win%') 
                      AND (mt.modality LIKE '%Solo%' OR mt.modality LIKE '%Duo%' OR mt.modality LIKE '%Superv%')";

                using (var cmdSurv = new MySqlCommand(sqlSurv, connection))
                {
                    cmdSurv.Parameters.AddWithValue("@id", id);
                    if (winsSurvivalText != null) winsSurvivalText.text = cmdSurv.ExecuteScalar().ToString();
                }
            }
            catch (System.Exception e) { Debug.LogError("Error en query de Victorias Supervivencia: " + e.Message); }


         
            try
            {
                string sqlStreak = @"
                    SELECT mu.result FROM MATCH_USER mu 
                    JOIN MATCHES m ON mu.match_id = m.match_id 
                    WHERE mu.user_id = @id 
                    ORDER BY m.date DESC";

                int streakCounter = 0;
                using (var cmdStreak = new MySqlCommand(sqlStreak, connection))
                {
                    cmdStreak.Parameters.AddWithValue("@id", id);
                    using (var readerStreak = cmdStreak.ExecuteReader())
                    {
                        while (readerStreak.Read())
                        {
                            string res = readerStreak.GetString("result").ToLower();
                            if (res.Contains("vict") || res.Contains("win"))
                            {
                                streakCounter++;
                            }
                            else
                            {
                                break; 
                            }
                        }
                    }
                }
                if (currentStreakText != null) currentStreakText.text = streakCounter.ToString();
            }
            catch (System.Exception e)
            {
                Debug.LogError(" Error al calcular Racha de Victorias: " + e.Message);
                if (currentStreakText != null) currentStreakText.text = "0";
            }


            
            try
            {
                string sqlFavB = @"
                    SELECT b.name FROM MATCH_USER mu 
                    JOIN BRAWLERS b ON mu.brawler_id = b.brawler_id 
                    WHERE mu.user_id = @id 
                    GROUP BY mu.brawler_id, b.name 
                    ORDER BY COUNT(*) DESC LIMIT 1";

                using (var cmdFavB = new MySqlCommand(sqlFavB, connection))
                {
                    cmdFavB.Parameters.AddWithValue("@id", id);
                    var res = cmdFavB.ExecuteScalar();
                    if (favoriteBrawlerText != null) favoriteBrawlerText.text = res?.ToString() ?? "N/A";
                }
            }
            catch (System.Exception e) { Debug.LogError(" Error en Brawler Favorito: " + e.Message); }


            
            try
            {
                string sqlWinB = @"
                    SELECT b.name FROM MATCH_USER mu 
                    JOIN BRAWLERS b ON mu.brawler_id = b.brawler_id 
                    WHERE mu.user_id = @id AND (mu.result LIKE '%vict%' OR mu.result LIKE '%win%')
                    GROUP BY mu.brawler_id, b.name 
                    ORDER BY COUNT(*) DESC LIMIT 1";

                using (var cmdWinB = new MySqlCommand(sqlWinB, connection))
                {
                    cmdWinB.Parameters.AddWithValue("@id", id);
                    var res = cmdWinB.ExecuteScalar();
                    if (winningestBrawlerText != null) winningestBrawlerText.text = res?.ToString() ?? "N/A";
                }
            }
            catch (System.Exception e) { Debug.LogError(" Error en Brawler con más Victorias: " + e.Message); }


            
            try
            {
                string sqlMode = @"
                    SELECT mt.name FROM MATCH_USER mu 
                    JOIN MATCHES m ON mu.match_id = m.match_id 
                    JOIN MATCH_TYPES mt ON m.match_type_id = mt.match_type_id 
                    WHERE mu.user_id = @id 
                    GROUP BY mt.match_type_id, mt.name 
                    ORDER BY COUNT(*) DESC LIMIT 1";

                using (var cmdMode = new MySqlCommand(sqlMode, connection))
                {
                    cmdMode.Parameters.AddWithValue("@id", id);
                    var res = cmdMode.ExecuteScalar();
                    if (favoriteModeText != null) favoriteModeText.text = res?.ToString() ?? "N/A";
                }
            }
            catch (System.Exception e) { Debug.LogError(" Error en Modo Favorito: " + e.Message); }


            
            try
            {
                string sqlLastBrawler = @"
                    SELECT ub.level, ub.trophies 
                    FROM MATCH_USER mu
                    JOIN MATCHES m ON mu.match_id = m.match_id
                    JOIN USER_BRAWLERS ub ON mu.user_id = ub.user_id AND mu.brawler_id = ub.brawler_id
                    WHERE mu.user_id = @userId
                    ORDER m.date DESC 
                    LIMIT 1";

                using (var cmdLastB = new MySqlCommand(sqlLastBrawler, connection))
                {
                    cmdLastB.Parameters.AddWithValue("@userId", id);
                    using (var reader = cmdLastB.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (lastBrawlerLevelText != null)
                                lastBrawlerLevelText.text = "Nivel " + reader.GetInt32("level").ToString();

                            if (lastBrawlerTrophiesText != null)
                                lastBrawlerTrophiesText.text = reader.GetInt32("trophies").ToString() + " ";
                        }
                        else
                        {
                            if (lastBrawlerLevelText != null) lastBrawlerLevelText.text = "N/A";
                            if (lastBrawlerTrophiesText != null) lastBrawlerTrophiesText.text = "0 ";
                        }
                    }
                }
            }
            catch (System.Exception e) { Debug.LogError(" Error en Último Brawler Usado: " + e.Message); }
        }
    }
}