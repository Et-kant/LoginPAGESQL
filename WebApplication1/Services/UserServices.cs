using MySqlConnector;
using WebApplication1.Data;
using WebApplication1.Utils;

namespace WebApplication1.Services
{
    public class UserServices
    {
        private readonly DbConnection _db;

        public UserServices(DbConnection db)
        {
            _db = db;
        }

        public bool Register(string username, string email, string password)
        {
            var HashedPassword = PasswordHasher.HashPassword(password);

            using (var conn = _db.GetConnection())
            {
                try
                {

                }
                catch
                {
                    return false;
                }
    }
}
