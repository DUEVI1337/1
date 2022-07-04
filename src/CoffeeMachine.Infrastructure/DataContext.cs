using System;

using CoffeeMachine.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace CoffeeMachine.Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opt) : base(opt)
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// <see cref="Balance"/> table in database
        /// </summary>
        public DbSet<Balance> Balances { get; set; }

        /// <summary>
        /// <see cref="BanknoteCashbox"/> table in database
        /// </summary>
        public DbSet<BanknoteCashbox> BanknoteCashboxes { get; set; }

        /// <summary>
        /// <see cref="Coffee"/> table in database
        /// </summary>
        public DbSet<Coffee> Coffees { get; set; }

        /// <summary>
        /// <see cref="Income"/> table in database
        /// </summary>
        public DbSet<Income> Incomes { get; set; }

        /// <summary>
        /// <see cref="Payment"/> table in database
        /// </summary>
        public DbSet<Payment> Payments { get; set; }

        /// <summary>
        /// init <see cref="Coffee"/>, <see cref="BanknoteCashbox"/> table in database 
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Coffee>().HasData(new Coffee { CoffeeId = Guid.NewGuid(), Name = "Капучино", Price = 600 },
                new Coffee { CoffeeId = Guid.NewGuid(), Name = "Латте", Price = 850 },
                new Coffee { CoffeeId = Guid.NewGuid(), Name = "Американо", Price = 900 });
            builder.Entity<BanknoteCashbox>().HasData(
                new BanknoteCashbox { BanknoteId = Guid.NewGuid(), Denomination = 50, CountBanknote = 50 },
                new BanknoteCashbox { BanknoteId = Guid.NewGuid(), Denomination = 100, CountBanknote = 40 },
                new BanknoteCashbox { BanknoteId = Guid.NewGuid(), Denomination = 200, CountBanknote = 30 },
                new BanknoteCashbox { BanknoteId = Guid.NewGuid(), Denomination = 500, CountBanknote = 20 },
                new BanknoteCashbox { BanknoteId = Guid.NewGuid(), Denomination = 1000, CountBanknote = 15 },
                new BanknoteCashbox { BanknoteId = Guid.NewGuid(), Denomination = 2000, CountBanknote = 10 },
                new BanknoteCashbox { BanknoteId = Guid.NewGuid(), Denomination = 5000, CountBanknote = 5 });
        }
    }
}