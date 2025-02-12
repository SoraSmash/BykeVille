namespace BykeVille.AuthModels
{
    public class Credentials
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public Credentials(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
