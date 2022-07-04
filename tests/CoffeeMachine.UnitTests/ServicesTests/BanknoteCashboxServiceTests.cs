using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using CoffeeMachine.Application.Services;
using CoffeeMachine.Domain.Dto;
using CoffeeMachine.Domain.Entities;
using CoffeeMachine.Infrastructure;
using CoffeeMachine.Infrastructure.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

namespace CoffeeMachine.UnitTests.ServicesTests
{
    public class BanknoteCashboxServiceTests
    {
        private BanknoteCashboxService _cashboxService;
        private DataContext _db;

        [SetUp]
        public void Setup()
        {
            var _dbOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _db = new DataContext(_dbOptions);
            _db.Database.EnsureDeleted();
            var uow = new UnitOfWork(_db, new CoffeeRepository(_db), new BalanceRepository(_db),
                new BanknoteCashboxRepository(_db), new PaymentRepository(_db), new IncomeRepository(_db));
            _cashboxService = new BanknoteCashboxService(uow);
        }

        [Test]
        public async Task GetCashboxAsync_AddCashboxInDb_ReturnSameCount()
        {
            //Arrange
            List<BanknoteCashbox> cashboxExpected = new ()
            {
                new BanknoteCashbox {BanknoteId = Guid.NewGuid(), Denomination = 100, CountBanknote = 10},
                new BanknoteCashbox {BanknoteId = Guid.NewGuid(), Denomination = 200, CountBanknote = 10},
            };
            _db.BanknoteCashboxes.AddRange(cashboxExpected);
            await _db.SaveChangesAsync();

            //Act
            List<BanknoteCashbox> cashboxActual = await _cashboxService.GetCashboxAsync();

            //Assert
            await _db.DisposeAsync();
            Assert.That(cashboxActual, Has.Count.EqualTo(cashboxExpected.Count));
        }

        [Test]
        public async Task UpdateCashboxAsync_CashboxFromDb_ReturnNewCashboxFromDb()
        {
            //Arrange
            List<BanknoteCashbox> cashboxInDb = new()
            {
                new BanknoteCashbox { BanknoteId = Guid.NewGuid(), CountBanknote = 10, Denomination = 100 },
                new BanknoteCashbox { BanknoteId = Guid.NewGuid(), CountBanknote = 10, Denomination = 200 }
            };
            _db.BanknoteCashboxes.AddRange(cashboxInDb);
            await _db.SaveChangesAsync();
            List<BanknoteCashbox> cashboxExpected = new()
            {
                new BanknoteCashbox {BanknoteId = cashboxInDb[0].BanknoteId, CountBanknote = 15, Denomination = 100},
                new BanknoteCashbox {BanknoteId = cashboxInDb[1].BanknoteId, CountBanknote = 20, Denomination = 200}
            };

            //Act
            await _cashboxService.UpdateCashboxAsync(cashboxExpected);
            await _db.SaveChangesAsync();

            //Assert
            var cashboxActual = _db.BanknoteCashboxes.ToList();
            await _db.DisposeAsync();
            cashboxActual.Should().BeEquivalentTo(cashboxExpected);
        }
    }
}
