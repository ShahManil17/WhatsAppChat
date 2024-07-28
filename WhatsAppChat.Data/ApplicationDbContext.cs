using Microsoft.EntityFrameworkCore;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .Property(p => p.IsDeleted)
                .HasDefaultValue(0);
            modelBuilder.Entity<Communication>()
                .Property(p => p.IsRead)
                .HasDefaultValue(0);
            modelBuilder.Entity<Communication>()
                .Property(p => p.IsDelivered)
                .HasDefaultValue(0);
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
        public DbSet<Communication> Communication { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<GroupHasMembers> GroupHasMembers { get; set; }
        public DbSet<GroupMessages> GroupMessages { get; set; }
		public DbSet<GroupUnreads> GroupUnreads { get; set; }
	}
}
