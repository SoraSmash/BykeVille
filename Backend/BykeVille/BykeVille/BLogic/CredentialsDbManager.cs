using BykeVille.AuthModels;
using BykeVille.Models;
using BykeVille.NewModels;
using Faker;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace BykeVille.BLogic
{
    public class CredentialsDbManager
    {
        private readonly SqlConnection _connection = new();
        private SqlCommand _command = new();
        public readonly bool IsDbOnline = false;
        private string connectionStringDbOld;
        private string connectionStringDbNew;

        public CredentialsDbManager(string connectionStringDbOld, string connectionStringDbNew)
        {
            try
            {
                this.connectionStringDbOld = connectionStringDbOld;
                this.connectionStringDbNew = connectionStringDbNew;
                _connection.ConnectionString = this.connectionStringDbOld;
                _connection.Open();
                IsDbOnline = true;
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        //Apre il Db
        private void CheckOpenedDB()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        //Chiude il Db
        private void CheckClosedDB(SqlDataReader dataReader)
        {
            dataReader?.Close();
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        //Cripta la password con il salt utilizzando SHA256
        public string Sha256Encrypt(string password, string salt)
        {
            try
            {
                var saltedPassword = Encoding.UTF8.GetBytes(salt + password);
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = sha256.ComputeHash(saltedPassword);
                    return Convert.ToBase64String(hashBytes);
                }
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return null;
            }
        }

        //Ottiene il salt della password dal database
        public string GetPasswordSalt(string emailAddress, string tableName)
        {

            string passwordSalt = null;

            try
            {
                CheckOpenedDB();
                _command = new SqlCommand($"SELECT [PasswordSalt] FROM " + tableName + " WHERE [EmailAddress] = @emailAddress", _connection);
                _command.Parameters.AddWithValue("@emailaddress", emailAddress);

                SqlDataReader dataReader = _command.ExecuteReader();
                while (dataReader.Read())
                {
                    passwordSalt = dataReader["PasswordSalt"].ToString();
                }

                CheckClosedDB(dataReader);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return passwordSalt;
        }

        //Controlla se l'utente è presente nel database utilizzando la sua e-mail e la sua password
        public System.Boolean CheckUserByEmailAndPassword(string emailAddress, string passwordHash, string tableName)
        {

            System.Boolean isChecked = false;

            try
            {
                CheckOpenedDB();
                _command = new SqlCommand($"SELECT * FROM " + tableName + " WHERE [EmailAddress] = @emailAddress AND [PasswordHash] = @passwordHash", _connection);
                _command.Parameters.AddWithValue("@emailAddress", emailAddress);
                _command.Parameters.AddWithValue("@passwordHash", passwordHash);

                SqlDataReader dataReader = _command.ExecuteReader();
                while (dataReader.Read())
                {
                    isChecked = true;
                }

                CheckClosedDB(dataReader);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return isChecked;
        }

        //Controlla se l'utente è presente nel database utilizzando il suo OldId
        public System.Boolean CheckUserByOldId(int Id)
        {
            System.Boolean isChecked = false;

            try
            {
                CheckOpenedDB();
                _command = new SqlCommand($"SELECT * FROM [SalesLT].[NewCustomer] WHERE [CustomerOldID] = @Id", _connection);
                _command.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dataReader = _command.ExecuteReader();
                while (dataReader.Read())
                {
                    isChecked = true;
                }

                CheckClosedDB(dataReader);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return isChecked;
        }

        //Ottiene un cliente dal database utilizzando la sua e-mail
        public Customer GetCustomer(string emailAddress, string tableName)
        {
            Customer customer = null;

            try
            {
                CheckOpenedDB();
                _command = new SqlCommand($"SELECT * FROM " + tableName + " WHERE [EmailAddress] = @emailAddress", _connection);
                _command.Parameters.AddWithValue("@emailAddress", emailAddress);

                SqlDataReader dataReader = _command.ExecuteReader();
                while (dataReader.Read())
                {
                    customer = new Customer(
                        Int32.Parse(dataReader["CustomerID"].ToString()),
                        Convert.ToBoolean(dataReader["NameStyle"]),
                        dataReader["Title"].ToString(),
                        dataReader["FirstName"].ToString(),
                        dataReader["MiddleName"].ToString(),
                        dataReader["LastName"].ToString(),
                        dataReader["Suffix"].ToString(),
                        dataReader["CompanyName"].ToString(),
                        dataReader["SalesPerson"].ToString(),
                        dataReader["EmailAddress"].ToString(),
                        dataReader["Phone"].ToString(),
                        dataReader["PasswordHash"].ToString(),
                        dataReader["PasswordSalt"].ToString(),
                        Guid.Parse(dataReader["rowguid"].ToString()),
                        DateTime.Parse(dataReader["ModifiedDate"].ToString())
                        );
                }

                CheckClosedDB(dataReader);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }
            return customer;
        }

        //Converte un cliente di DbBikeVilleOld in un cliente di DbBikeVilleNew
        public NewCustomer ConvertCustomerToNewCustomer(Customer customer)
        {
            NewCustomer newCustomer = null;

            try
            {
                newCustomer = new NewCustomer(
                    customer.CustomerId,
                    customer.NameStyle,
                    customer.Title,
                    customer.FirstName,
                    customer.MiddleName,
                    customer.LastName,
                    customer.Suffix,
                    customer.CompanyName,
                    customer.SalesPerson,
                    customer.EmailAddress,
                    customer.Phone,
                    customer.PasswordHash,
                    customer.PasswordSalt,
                    customer.Rowguid,
                    customer.ModifiedDate,
                    "user",
                    customer.CustomerId
                    );
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return newCustomer;
        }

        //Converte una lista di indirizzi di DbBikeVilleOld in una lista di indirizzi di DbBikeVilleNew
        public List<NewAddress> ConvertAddressToNewAddress(List<Models.Address> addresses)
        {
            List<NewAddress> newAddresses = new List<NewAddress>();

            try
            {
                foreach (Models.Address address in addresses)
                {
                    newAddresses.Add(new NewAddress(
                        address.AddressId,
                        address.AddressLine1,
                        address.AddressLine2,
                        address.City,
                        address.StateProvince,
                        address.CountryRegion,
                        address.PostalCode,
                        address.Rowguid,
                        address.ModifiedDate,
                        address.AddressId
                        ));
                }
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return newAddresses;
        }

        //Converte una lista di CustomerAddress di DbBikeVilleOld in una lista di NewCustomerAddress di DbBikeVilleNew
        public List<NewCustomerAddress> ConvertCustomerAddressToNewCustomerAddress(List<CustomerAddress> customerAddresses)
        {
            List<NewCustomerAddress> newCustomerAddresses = new List<NewCustomerAddress>();

            try
            {
                foreach (CustomerAddress customerAddress in customerAddresses)
                {
                    newCustomerAddresses.Add(new NewCustomerAddress(
                        customerAddress.CustomerId,
                        customerAddress.AddressId,
                        customerAddress.AddressType,
                        customerAddress.Rowguid,
                        customerAddress.ModifiedDate,
                        customerAddress.CustomerId,
                        customerAddress.AddressId
                        ));
                }
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return newCustomerAddresses;
        }

        //Aggiunge un nuovo cliente in DbBikeVilleNew
        public void AddNewCustomer(NewCustomer newCustomer)
        {
            try
            {

                CheckOpenedDB();
                _command = new SqlCommand("InsertNewCustomer", _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@NameStyle", newCustomer.NameStyle);
                _command.Parameters.AddWithValue("@Title", newCustomer.Title);
                _command.Parameters.AddWithValue("@FirstName", newCustomer.FirstName);
                _command.Parameters.AddWithValue("@MiddleName", newCustomer.MiddleName);
                _command.Parameters.AddWithValue("@LastName", newCustomer.LastName);
                _command.Parameters.AddWithValue("@Suffix", newCustomer.Suffix);
                _command.Parameters.AddWithValue("@CompanyName", newCustomer.CompanyName);
                _command.Parameters.AddWithValue("@SalesPerson", newCustomer.SalesPerson);
                _command.Parameters.AddWithValue("@EmailAddress", newCustomer.EmailAddress);
                _command.Parameters.AddWithValue("@Phone", newCustomer.Phone);
                _command.Parameters.AddWithValue("@PasswordHash", newCustomer.PasswordHash);
                _command.Parameters.AddWithValue("@PasswordSalt", newCustomer.PasswordSalt);
                _command.Parameters.AddWithValue("@rowguid", newCustomer.Rowguid);
                _command.Parameters.AddWithValue("@ModifiedDate", newCustomer.ModifiedDate);
                _command.Parameters.AddWithValue("@Role", newCustomer.Role);
                _command.Parameters.AddWithValue("@CustomerOldID", newCustomer.CustomerOldId);
                _command.ExecuteNonQuery();
                CheckClosedDB(null);

            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }
        }

        //Aggiunge un nuovo indirizzo in DbBikeVilleNew
        public void AddNewAddress(NewAddress newAddress)
        {
            try
            {

                CheckOpenedDB();
                _command = new SqlCommand("InsertNewAddress", _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@AddressLine1", newAddress.AddressLine1);
                _command.Parameters.AddWithValue("@AddressLine2", newAddress.AddressLine2);
                _command.Parameters.AddWithValue("@City", newAddress.City);
                _command.Parameters.AddWithValue("@StateProvince", newAddress.StateProvince);
                _command.Parameters.AddWithValue("@CountryRegion", newAddress.CountryRegion);
                _command.Parameters.AddWithValue("@PostalCode", newAddress.PostalCode);
                _command.Parameters.AddWithValue("@rowguid", newAddress.Rowguid);
                _command.Parameters.AddWithValue("@ModifiedDate", newAddress.ModifiedDate);
                _command.Parameters.AddWithValue("@AddressOldID", newAddress.AddressOldId);
                _command.ExecuteNonQuery();
                CheckClosedDB(null);

            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }
        }

        //Aggiunge un nuovo CustomerAddress in DbBikeVilleNew
        public void AddNewCustomerAddress(NewCustomerAddress newCustomerAddress)
        {
            try
            {

                CheckOpenedDB();
                _command = new SqlCommand("InsertNewCustomerAddress", _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@CustomerID", newCustomerAddress.CustomerId);
                _command.Parameters.AddWithValue("@AddressID", newCustomerAddress.AddressId);
                _command.Parameters.AddWithValue("@AddressType", newCustomerAddress.AddressType);
                _command.Parameters.AddWithValue("@rowguid", newCustomerAddress.Rowguid);
                _command.Parameters.AddWithValue("@ModifiedDate", newCustomerAddress.ModifiedDate);
                _command.Parameters.AddWithValue("@CustomerOldID", newCustomerAddress.CustomerOldId);
                _command.Parameters.AddWithValue("@AddressOldID", newCustomerAddress.AddressOldId);
                _command.ExecuteNonQuery();
                CheckClosedDB(null);

            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }
        }

        //Genera una stringa casuale di lunghezza length con i caratteri presenti in characters
        public string GenerateRandomString(int length, string characters)
        {
            Random random = new Random();
            string result = "";

            try
            {
                for (int i = 0; i < length; i++)
                {
                    result = result + characters[random.Next(characters.Length)];
                }
            }
            catch(Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return result;
        }

        //Genera una GUID casuale
        public Guid GenerateRowGuid(string tableName)
        {
            string characters = "ABCDEF0123456789";
            Guid rowGuid = Guid.Parse($"{GenerateRandomString(8, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(12, characters)}"); ;
            System.Boolean isFree = false;


            try
            {
                while (!isFree)
                {
                
                    rowGuid = Guid.Parse($"{GenerateRandomString(8, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(12, characters)}");

                    CheckOpenedDB();
                    _command = new SqlCommand($"SELECT * FROM " + tableName + " WHERE [rowguid] = @rowGuid", _connection);
                    _command.Parameters.AddWithValue("@rowGuid", rowGuid);

                    isFree = true;
                    SqlDataReader dataReader = _command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        isFree = false;
                    }

                    CheckClosedDB(dataReader);
                }
                
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return rowGuid;
        }

        //Aggiorna il cliente in DbBikeVilleOld inserendo dati fasulli
        public void UpdateFakeCustomer(NewCustomer newCustomer)
        {
            try
            {
                Random random = new Random();

                CultureInfo.CurrentUICulture = new CultureInfo("en-US");

                string title = Faker.Name.Prefix();
                string firstName = Faker.Name.First();
                string lastName = Faker.Name.Last();
                string suffix = Faker.Name.Suffix();
                string companyName = Company.Name();
                string salesPerson = "adventure-works\\" + Faker.Name.First() + random.Next(0, 10);
                string emailAddress = Internet.Email();
                string phone = Phone.Number().Split(" x")[0].Replace('.', '-');
                if (phone.Length > 12)
                    phone = phone.Substring(phone.Length - 12);
                string password = Faker.Name.First();
                string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                string passwordSalt = GenerateRandomString(7, characters);
                passwordSalt += "=";
                string passwordHash = Sha256Encrypt(password, passwordSalt);
                string rowguid = GenerateRowGuid("[SalesLT].[Customer]").ToString();

                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;


                CheckOpenedDB();
                _command = new SqlCommand("UpdateCustomer", _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@CustomerID", newCustomer.CustomerOldId);
                _command.Parameters.AddWithValue("@NameStyle", newCustomer.NameStyle);
                _command.Parameters.AddWithValue("@Title", title);
                _command.Parameters.AddWithValue("@FirstName", firstName);
                _command.Parameters.AddWithValue("@MiddleName", newCustomer.MiddleName);
                _command.Parameters.AddWithValue("@LastName", lastName);
                _command.Parameters.AddWithValue("@Suffix", suffix);
                _command.Parameters.AddWithValue("@CompanyName", companyName);
                _command.Parameters.AddWithValue("@SalesPerson", salesPerson);
                _command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                _command.Parameters.AddWithValue("@Phone", phone);
                _command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                _command.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
                _command.Parameters.AddWithValue("@rowguid", rowguid);
                _command.Parameters.AddWithValue("@ModifiedDate", newCustomer.ModifiedDate);
                _command.ExecuteNonQuery();
                CheckClosedDB(null);

            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }
        }

        //Aggiorna l'indirizzo in DbBikeVilleOld inserendo dati fasulli
        public void UpdateFakeAddress(NewAddress newAddress)
        {
            try
            {
                Random random = new Random();

                CultureInfo.CurrentUICulture = new CultureInfo("en-US");

                string addressLine1 = Faker.Address.StreetAddress();
                string city = Faker.Address.City();
                string stateProvince = Faker.Address.Country();
                string countryRegion = "United States";
                string postalCode = Faker.Address.ZipCode();
                string password = Faker.Name.First();
                string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                string passwordSalt = GenerateRandomString(7, characters);
                passwordSalt += "=";
                string passwordHash = Sha256Encrypt(password, passwordSalt);
                string rowguid = GenerateRowGuid("[SalesLT].[Customer]").ToString();

                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;


                CheckOpenedDB();
                _command = new SqlCommand("UpdateAddress", _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@AddressID", newAddress.AddressId);
                _command.Parameters.AddWithValue("@AddressLine1", addressLine1);
                _command.Parameters.AddWithValue("@AddressLine2", newAddress.AddressLine2);
                _command.Parameters.AddWithValue("@City", city);
                _command.Parameters.AddWithValue("@StateProvince", stateProvince);
                _command.Parameters.AddWithValue("@CountryRegion", countryRegion);
                _command.Parameters.AddWithValue("@PostalCode", postalCode);
                _command.Parameters.AddWithValue("@rowguid", rowguid);
                _command.Parameters.AddWithValue("@ModifiedDate", newAddress.ModifiedDate);
                _command.ExecuteNonQuery();
                CheckClosedDB(null);

            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }
        }

        //Inserisce un cliente fasullo in DbBikeVilleOld
        public int AddFakeCustomer()
        {
            int customerId = -1;

            try
            {
                Random random = new Random();

                CultureInfo.CurrentUICulture = new CultureInfo("en-US");

                string title = Faker.Name.Prefix();
                string firstName = Faker.Name.First();
                string lastName = Faker.Name.Last();
                string suffix = Faker.Name.Suffix();
                string companyName = Company.Name();
                string salesPerson = "adventure-works\\" + Faker.Name.First() + random.Next(0, 10);
                string emailAddress = Internet.Email();
                string phone = Phone.Number().Split(" x")[0].Replace('.', '-');
                if(phone.Length > 12)
                    phone = phone.Substring(phone.Length - 12);
                string password = Faker.Name.First();
                string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                string passwordSalt = GenerateRandomString(7, characters);
                passwordSalt += "=";
                string passwordHash = Sha256Encrypt(password, passwordSalt);
                string rowguid = GenerateRowGuid("[SalesLT].[Customer]").ToString();

                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;


                CheckOpenedDB();
                _command = new SqlCommand("InsertCustomer", _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@NameStyle", 0);
                _command.Parameters.AddWithValue("@Title", title);
                _command.Parameters.AddWithValue("@FirstName", firstName);
                _command.Parameters.AddWithValue("@MiddleName", DBNull.Value);
                _command.Parameters.AddWithValue("@LastName", lastName);
                _command.Parameters.AddWithValue("@Suffix", suffix);
                _command.Parameters.AddWithValue("@CompanyName", companyName);
                _command.Parameters.AddWithValue("@SalesPerson", salesPerson);
                _command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                _command.Parameters.AddWithValue("@Phone", phone);
                _command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                _command.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
                _command.Parameters.AddWithValue("@rowguid", rowguid);
                _command.Parameters.AddWithValue("@ModifiedDate", DateTime.Now.Date);

                SqlParameter customerIdParam = new SqlParameter("@CustomerID", SqlDbType.Int);
                customerIdParam.Direction = ParameterDirection.Output;
                _command.Parameters.Add(customerIdParam);

                _command.ExecuteNonQuery();

                customerId = (int)customerIdParam.Value;
                
                CheckClosedDB(null);

            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return customerId;
        }

        //Ottiene gli indirizzi di un cliente dal database DbBikeVilleOld
        public List<Models.Address> GetAddressesByCustomerId(int customerId)
        {
            List<Models.Address> addresses = new();

            try
            {
                CheckOpenedDB();
                _command = new SqlCommand($"SELECT a.AddressID, a.AddressLine1, a.AddressLine2, a.City, a.StateProvince, a.CountryRegion, a.PostalCode, a.rowguid, a.ModifiedDate FROM [DbBikeVilleOld].[SalesLT].[Customer] c JOIN [DbBikeVilleOld].[SalesLT].[CustomerAddress] ca ON c.CustomerID = ca.CustomerID JOIN [DbBikeVilleOld].[SalesLT].[Address] a ON ca.AddressID = a.AddressID WHERE c.CustomerID = @customerId", _connection);
                _command.Parameters.AddWithValue("@customerId", customerId);

                SqlDataReader dataReader = _command.ExecuteReader();
                while (dataReader.Read())
                {
                    addresses.Add(new Models.Address(
                        Int32.Parse(dataReader["AddressID"].ToString()),
                        dataReader["AddressLine1"].ToString(),
                        dataReader["AddressLine2"].ToString(),
                        dataReader["City"].ToString(),
                        dataReader["StateProvince"].ToString(),
                        dataReader["CountryRegion"].ToString(),
                        dataReader["PostalCode"].ToString(),
                        Guid.Parse(dataReader["rowguid"].ToString()),
                        DateTime.Parse(dataReader["ModifiedDate"].ToString())
                        ));
                }

                CheckClosedDB(dataReader);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }
            return addresses;
        }

        //Ottiene i CustomerAddresses di un cliente dal database DbBikeVilleOld
        public List<CustomerAddress> GetCustomerAddressesByCustomerId(int customerId)
        {
            List<CustomerAddress> customerAddresses = new();

            try
            {
                CheckOpenedDB();
                _command = new SqlCommand($"SELECT ca.CustomerID, ca.AddressID, ca.AddressType, ca.rowguid, ca.ModifiedDate FROM [DbBikeVilleOld].[SalesLT].[CustomerAddress] ca WHERE ca.CustomerID = @customerId", _connection);
                _command.Parameters.AddWithValue("@customerId", customerId);

                SqlDataReader dataReader = _command.ExecuteReader();
                while (dataReader.Read())
                {
                    customerAddresses.Add(new CustomerAddress(
                        Int32.Parse(dataReader["CustomerID"].ToString()),
                        Int32.Parse(dataReader["AddressID"].ToString()),
                        dataReader["AddressType"].ToString(),
                        Guid.Parse(dataReader["rowguid"].ToString()),
                        DateTime.Parse(dataReader["ModifiedDate"].ToString())
                        ));
                }

                CheckClosedDB(dataReader);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }
            return customerAddresses;
        }

        //Controlla se un utente è presente in uno dei due database:
        //Caso 1 - l'utente è presente in DbBikeVilleOld: inserisce dati fasulli nella sua riga, copia i dati originali in DbBikeVilleNew e restituisce true
        //Caso 2 - l'utente è presente in DbBikeVilleNew: restituisce true
        //Caso 3 - l'utente non è presente in nessun database: restituisce false
        public System.Boolean CheckCredentials(Credentials credentials)
        {
            
            System.Boolean isChecked = false;
            string passwordHash;
            string passwordSalt = null;
            string customerTable = "[SalesLT].[Customer]";
            string newCustomerTable = "[SalesLT].[NewCustomer]";
            Customer customer = null;
            List<Models.Address> addresses = new List<Models.Address> ();
            List<CustomerAddress> customerAddresses = new List<CustomerAddress>();

            try {

                _connection.ConnectionString = connectionStringDbOld;

                //Ottiene il salt della password dal DbBikeVilleOld
                passwordSalt = GetPasswordSalt(credentials.Email, customerTable);

                if (passwordSalt != null)
                {
                    //Cripta la password con il salt
                    passwordHash = Sha256Encrypt(credentials.Password, passwordSalt);

                    //Controlla se l'utente è presente in DbBikeVilleOld
                    isChecked = CheckUserByEmailAndPassword(credentials.Email, passwordHash, customerTable);

                    //Se l'utente è presente in DbBikeVilleOld
                    if (isChecked)
                    {
                        //Ottiene i dati sulle tabelle Customer, Address e CustomerAddress di quel cliente in DbBikeVilleOld
                        customer = GetCustomer(credentials.Email, customerTable);
                        addresses = GetAddressesByCustomerId(customer.CustomerId);
                        customerAddresses = GetCustomerAddressesByCustomerId(customer.CustomerId);

                        //Converte i dati sul cliente in modo da poterli inserire in DbBikeVilleNew
                        NewCustomer newCustomer = ConvertCustomerToNewCustomer(customer);
                        List<NewAddress> newAddresses = ConvertAddressToNewAddress(addresses);
                        List<NewCustomerAddress> newCustomerAddresses = ConvertCustomerAddressToNewCustomerAddress(customerAddresses);

                        _connection.ConnectionString = connectionStringDbNew;

                        //Controlla se l'utente è presente in DbBikeVilleNew.
                        //In questo modo si impedisce a eventuali attaccanti che siano riusciti a ottenere credenziali false
                        //di poter comunque effettuare il login. Ad esempio:
                        // - Utente topo@lino.com con password Minnie è presente in DbBikeVilleNew.
                        // - Utente pape@rino.com con password Paperina è il Fake utente di topo@lino.com in DbBikeVilleOld.
                        // - Se l'attaccante prova a fare il login con pape@rino.com e password Paperina, fallirà il login.
                        if (!CheckUserByOldId(newCustomer.CustomerOldId))
                        {
                            //Aggiungo il cliente in DbBikeVilleNew
                            AddNewCustomer(newCustomer);
                            foreach(NewAddress newAddress in newAddresses)
                            {
                                AddNewAddress(newAddress);
                            }
                            foreach(NewCustomerAddress newCustomerAddress in newCustomerAddresses)
                            {
                                AddNewCustomerAddress(newCustomerAddress);
                            }
                            _connection.ConnectionString = connectionStringDbOld;
                            //Modifico i dati del cliente in DbBikeVilleOld
                            UpdateFakeCustomer(newCustomer);
                            foreach (NewAddress newAddress in newAddresses)
                            {
                                UpdateFakeAddress(newAddress);
                            }
                        }
                        else
                        {
                            isChecked = false;
                        }

                    }
                }

                //Se l'utente non è presente in DbBikeVilleOld
                if (!isChecked)
                {
                    _connection.ConnectionString = connectionStringDbNew;
                    passwordSalt = null;

                    //Ottiene il salt della password dal DbBikeVilleNew
                    passwordSalt = GetPasswordSalt(credentials.Email, newCustomerTable);

                    if (passwordSalt != null)
                    {
                        //Cripta la password con il salt
                        passwordHash = Sha256Encrypt(credentials.Password, passwordSalt);

                        //Controlla se l'utente è presente in DbBikeVilleNew
                        isChecked = CheckUserByEmailAndPassword(credentials.Email, passwordHash, newCustomerTable);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return isChecked;
        }

        //Aggiunge un nuovo utente nel database:
        //1 - Aggiunge un nuovo utente fasullo in DbBikeVilleOld
        //2 - Ottiene l'id del cliente appena aggiunto in DbBikeVilleOld
        //3 - Aggiunge il cliente in DbBikeVilleNew usando dati reali e usando come OldId l'id del cliente appena aggiunto in DbBikeVilleOld
        public System.Boolean AddCustomer(Customer customer)
        {
            int customerId;
            NewCustomer newCustomer;
            System.Boolean isChecked = false;
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string customerTable = "[SalesLT].[Customer]";
            string newCustomerTable = "[SalesLT].[NewCustomer]";
            _connection.ConnectionString = connectionStringDbOld;

            try
            {
                //Aggiunge un cliente fasullo in DbBikeVilleOld
                customerId = AddFakeCustomer();

                _connection.ConnectionString = connectionStringDbNew;

                customer.CustomerId = customerId;
                customer.PasswordSalt = GenerateRandomString(7, characters) + '=';
                customer.PasswordHash = Sha256Encrypt(customer.PasswordHash, customer.PasswordSalt);

                customer.Rowguid = GenerateRowGuid(newCustomerTable);
                customer.ModifiedDate = DateTime.Now.Date;
                //Converte il customer in un NewCustomer
                newCustomer = ConvertCustomerToNewCustomer(customer);

                //Aggiunge il cliente in DbBikeVilleNew
                AddNewCustomer(newCustomer);

                isChecked= true;
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return isChecked;
        }

        public System.Boolean CheckPassword(Credentials credentials)
        {
            string newCustomerTable = "[SalesLT].[NewCustomer]";
            string passwordSalt = null;
            System.Boolean isChecked = false;
            string passwordHash;

            _connection.ConnectionString = connectionStringDbNew;

            try
            {
                //Ottiene il salt della password dal DbBikeVilleNew
                passwordSalt = GetPasswordSalt(credentials.Email, newCustomerTable);

                //Cripta la password con il salt
                passwordHash = Sha256Encrypt(credentials.Password, passwordSalt);

                //Controlla se l'utente è presente in DbBikeVilleNew
                isChecked = CheckUserByEmailAndPassword(credentials.Email, passwordHash, newCustomerTable);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }

            return isChecked;
        }
    }
}
