using Microsoft.EntityFrameworkCore;
using FnbReservationAPI.src.features.User;
using FnbReservationAPI.src.features.Staff;
using FnbReservationAPI.src.features.Outlet;
using FnbReservationAPI.src.features.Reservation;
using FnbReservationAPI.src.features.Queue;
using FnbReservationAPI.src.features.BlackList;
using FnbReservationAPI.src.features.Notification;

namespace FnbReservationAPI.src
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        // Define DbSet properties for each model
        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Outlet> Outlets { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Queue> Queues { get; set; }
        public DbSet<BlackList> BlackLists { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}