using UnityEngine;
using MySqlConnector;
using UnityEditor;

public class Prueba : MonoBehaviour
{
    string servidor = "127.0.0.1";
    string bd = "brawl_stars";
    string usuario = "root";
    string pass = "root";
    void Start()
    {
        string cadenaConexion = $"Server={servidor}; Database={bd}; User ID={usuario}; Password={pass}; SslMode=None";
        using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
        {
            conexion.Open();

            string sql = "SELECT * From brawlers";
            MySqlCommand cmd = new MySqlCommand(sql, conexion);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string nombre = reader.GetString(1).ToString();
                    Debug.Log(nombre);
                }
            }

            conexion.Close();
        }
    }
}
