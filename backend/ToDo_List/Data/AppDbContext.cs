using Microsoft.EntityFrameworkCore;

namespace ToDo_List.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER CONFIG
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.HasIndex(u => u.Email)
                      .IsUnique(); 

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(u => u.IsDeleted)
                      .HasDefaultValue(false);

                entity.HasMany(u => u.Tasks)
                      .WithOne(t => t.User)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // TASK CONFIG
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.TaskId);

                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.Description)
                      .HasMaxLength(500);

                entity.Property(t => t.Priority)
                      .IsRequired();

                entity.Property(t => t.Status)
                      .IsRequired();

                entity.Property(t => t.Type)
                      .IsRequired(false);

                entity.Property(t => t.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(t => t.UpdatedAt)
                      .IsRequired(false); 

                entity.Property(t => t.CompletedAt)
                      .IsRequired(false); 

                entity.Property(t => t.IsDeleted)
                      .HasDefaultValue(false);
            });

            // GLOBAL SOFT DELETE FILTER
            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<TaskItem>()
                .HasQueryFilter(t => !t.IsDeleted);
        }
    }
}