using Microsoft.EntityFrameworkCore;

namespace IadeFormu.Data
{
    public class MagazaBilgiDbContext : DbContext
    {
        public MagazaBilgiDbContext(DbContextOptions<MagazaBilgiDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public string Magaza { get; set; }
    }
}
