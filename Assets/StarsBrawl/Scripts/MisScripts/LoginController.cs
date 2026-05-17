using UnityEngine;
using TMPro;
using MySqlConnector;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    public TMP_InputField idInput;
    
    private string connectionString = "Server=127.0.0.1; Database=brawl_stars; User ID=root; Password=root; SslMode=None;";

    public void OnConfirmClick()
    {
        string inputID = idInput.text;

        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string query = "SELECT user_id FROM USERS WHERE user_id = @id";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", inputID);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        
                        UserSession.CurrentUserID = inputID;
                        SceneManager.LoadScene("MainMenu");
                    }
                    else
                    {
                        Debug.LogError("El ID no existe en la base de datos.");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error de conexión: " + e.Message);
            }
        }
    }
}