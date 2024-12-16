using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CoreSystem2024.Models
{

    public class UsersContext : DbContext
    {
        public UsersContext(string connString)
            : base(GetOptions(connString))
        {
        }

        private static DbContextOptions GetOptions(string connString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connString).Options;
        }
        public DbSet<UserSession> UserSession { get; set; }


    }

    [Table("UserSession")]
    public class UserSession
    {
        [Key]
        public Guid SessionGuid { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int ActiveOrderId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
