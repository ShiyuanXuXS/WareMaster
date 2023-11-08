using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WareMaster
{
    public partial class User
    {
        public RoleEnum RoleEnum { get; set; }

        public override string ToString()
        {
            return $"User ID: {id}, Username: {Username}, Role: {RoleEnum}, Email: {Email}";
        }

        public void SetHashedPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedBytes = sha256.ComputeHash(bytes);
                Password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }

}
