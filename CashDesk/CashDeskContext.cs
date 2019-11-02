using System;
using System.Collections.Generic;
using System.Text;
using static CashDesk.Model;
using Microsoft.EntityFrameworkCore;

namespace CashDesk
{
    class CashDeskContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public DbSet<Membership> Memberships { get; set; }

        public DbSet<Deposit> Deposits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("cashdesk");            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>().HasIndex(m => m.LastName);
            modelBuilder.Entity<Membership>().HasIndex(ms => ms.Member);
        }
    }
}
