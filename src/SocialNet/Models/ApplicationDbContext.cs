using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using SocialNet.Models;
using Microsoft.Data.Entity.Metadata;

namespace SocialNet.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<Subscription>()
                .HasKey(s => new { s.PublisherId, s.SubscriberId });

            builder.Entity<Subscription>()
                .HasOne(s => s.Publisher)
                .WithMany(au => au.Subscribers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(s => s.PublisherId);

            builder.Entity<Subscription>()
                .HasOne(s => s.Subscriber)
                .WithMany(au => au.Publishers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(s => s.SubscriberId);
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
