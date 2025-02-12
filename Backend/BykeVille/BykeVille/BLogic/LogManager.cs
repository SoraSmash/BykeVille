using BykeVille.NewModels;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace BykeVille.BLogic
{
    public static class LogManager
    {
        private static readonly SqlConnection _connection = new();
        private static SqlCommand _command = new();
        public static readonly bool IsDbOnline = false;

        //Inizializza la connessione al database
        public static void InitConnection(string dbConnection)
        {
            try
            {
                _connection.ConnectionString = dbConnection;
                _connection.Open();
            }
            catch (Exception ex)
            {
                SaveLogBackend(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        //Apre la connessione al database
        private static void CheckOpenedDB()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        //Chiude la connessione al database
        private static void CheckClosedDB(SqlDataReader dataReader)
        {
            dataReader?.Close();
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        //Quando si verifica un'eccezione nel backend, salva i dettagli nel database
        public static void SaveLogBackend(Exception ex)
        {
            CheckOpenedDB();
            _command = new SqlCommand("InsertLogBackend", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.AddWithValue("@Date", DateTime.Now.ToString("dd-MM-yyyy"));
            _command.Parameters.AddWithValue("@Time", DateTime.Now.ToString("T"));
            _command.Parameters.AddWithValue("@Class", new StackTrace(ex).GetFrame(0).GetMethod().ReflectedType.Name);
            _command.Parameters.AddWithValue("@Method", new StackTrace(ex).GetFrame(0).GetMethod().Name);
            _command.Parameters.AddWithValue("@Exception", ex.ToString());
            _command.ExecuteNonQuery();
            CheckClosedDB(null);
        }

        //Carica i log del backend dal database
        public static List<LogBackend> LoadLogBackend(DateOnly date)
        {
            List<LogBackend> logs = new();

            try
            {
                CheckOpenedDB();
                _command = new SqlCommand($"SELECT * FROM [dbo].[Log.Backend] WHERE [dbo].[Log.Backend].[Date] = @Date", _connection);
                _command.Parameters.AddWithValue("@Date", date);
                SqlDataReader dataReader = _command.ExecuteReader();
                while (dataReader.Read())
                {
                    logs.Add(new LogBackend(
                        int.Parse(dataReader["ID"].ToString()),
                        DateOnly.Parse(dataReader["Date"].ToString().Split(' ')[0]),
                        TimeOnly.Parse(dataReader["Time"].ToString()),
                        dataReader["Class"].ToString(),
                        dataReader["Method"].ToString(),
                        dataReader["Exception"].ToString()
                        ));
                }
                CheckClosedDB(dataReader);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return logs;

        }

        //Quando si verifica un'eccezione nel frontend, salva i dettagli nel database
        public static void SaveLogFrontend(string exception)
        {
            try
            {
                CheckOpenedDB();
                _command = new SqlCommand("InsertLogFrontend", _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@Date", DateTime.Now.ToString("dd-MM-yyyy"));
                _command.Parameters.AddWithValue("@Time", DateTime.Now.ToString("T"));
                _command.Parameters.AddWithValue("@Exception", exception);
                _command.ExecuteNonQuery();
                CheckClosedDB(null);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }
        }

        //Carica i log del frontend dal database
        public static List<LogFrontend> LoadLogFrontend(DateOnly date)
        {
            List<LogFrontend> logs = new();

            try
            {
                CheckOpenedDB();
                _command = new SqlCommand($"SELECT * FROM [dbo].[Log.Frontend] WHERE [dbo].[Log.Frontend].[Date] = @Date", _connection);
                _command.Parameters.AddWithValue("@Date", date);
                SqlDataReader dataReader = _command.ExecuteReader();
                while (dataReader.Read())
                {
                    logs.Add(new LogFrontend(
                        int.Parse(dataReader["ID"].ToString()),
                        DateOnly.Parse(dataReader["Date"].ToString().Split(' ')[0]),
                        TimeOnly.Parse(dataReader["Time"].ToString()),
                        dataReader["Exception"].ToString()
                        ));
                }
                CheckClosedDB(dataReader);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return logs;

        }
    }
}
