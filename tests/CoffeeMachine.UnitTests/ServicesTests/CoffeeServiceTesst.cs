using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CoffeeMachine.Application.Mappers;
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
    public class CoffeeServiceTesst
    {
        private CoffeeService _coffeeService;
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
            _coffeeService = new CoffeeService(uow, new BanknoteCashboxService(uow), new BalanceService(uow), new PaymentService(uow), new IncomeService(uow));
        }

        [Test]
        public async Task GetListCoffeeDtoAsync_AddListCoffeeInDb_ReturnCoffeeDto()
        {
            //Arrange
            List<Coffee> coffeeInDb = new ()
            {
                new Coffee {CoffeeId = Guid.NewGuid(), Name = "Very Black", Price = 100},
                new Coffee {CoffeeId = Guid.NewGuid(), Name = "Latte", Price = 200},
            };
            _db.Coffees.AddRange(coffeeInDb);
            await _db.SaveChangesAsync();
            List<CoffeeDto> coffeesDtoExpected = new()
            {
                new CoffeeDto {CoffeeId = coffeeInDb[0].CoffeeId.ToString(), CoffeeName = coffeeInDb[0].Name, CoffeePrice = coffeeInDb[0].Price},
                new CoffeeDto {CoffeeId = coffeeInDb[1].CoffeeId.ToString(), CoffeeName = coffeeInDb[1].Name, CoffeePrice = coffeeInDb[1].Price}
            };

            //Act
            List<CoffeeDto> coffeesDtoActual = await _coffeeService.GetListCoffeeDtoAsync();

            //Assert
            await _db.DisposeAsync();
            coffeesDtoActual.Should().BeEquivalentTo(coffeesDtoExpected);
        }

        [Test]
        public async Task GetCoffeeDtoByIdAsync_AddCoffeeInDb_ReturnCoffeeDto()
        {
            //Arrange
            List<Coffee> coffeeInDb = new()
            {
                new Coffee {CoffeeId = Guid.NewGuid(), Name = "Very Black", Price = 100},
                new Coffee {CoffeeId = Guid.NewGuid(), Name = "Latte", Price = 200},
            };
            _db.Coffees.AddRange(coffeeInDb);
            await _db.SaveChangesAsync();
            CoffeeDto coffeeDtoExpected = new()
            {
                CoffeeId = coffeeInDb[0].CoffeeId.ToString(), CoffeePrice = coffeeInDb[0].Price, CoffeeName = coffeeInDb[0].Name
            };

            //Act
            CoffeeDto coffeeDtoActual = await _coffeeService.GetCoffeeDtoByIdAsync(coffeeInDb[0].CoffeeId.ToString());

            //Assert
            await _db.DisposeAsync();
            coffeeDtoActual.Should().BeEquivalentTo(coffeeDtoExpected);
        }

        [Test]
        public async Task BuyCoffee_CheckDeal_ReturnCorrectDeal()
        {
            //Arrange
            Coffee coffee = new()
            {
                Price = 100, CoffeeId = Guid.NewGuid(), Name = "Latte" 
            };
            List<BanknoteDto> clientMoney = new()
            {
                new BanknoteDto { CountBanknote = 1, Denomination = 500 },
                new BanknoteDto { CountBanknote = 1, Denomination = 1000 },
            };
            List<BanknoteCashbox> cashbox = new()
            {
                new BanknoteCashbox { CountBanknote = 30, Denomination = 100, BanknoteId = Guid.NewGuid() },
                new BanknoteCashbox { CountBanknote = 20, Denomination = 200, BanknoteId = Guid.NewGuid() },
                new BanknoteCashbox { CountBanknote = 10, Denomination = 500, BanknoteId = Guid.NewGuid() },
                new BanknoteCashbox { CountBanknote = 5, Denomination = 1000, BanknoteId = Guid.NewGuid() }
            };
            var typeDeal = TypeDeal.BigDeal;
            _db.Coffees.Add(coffee);
            _db.BanknoteCashboxes.AddRange(cashbox);
            await _db.SaveChangesAsync();
            const int amountDealExpected = 1400;

            //Act
            List<BanknoteDto> deal = await _coffeeService.BuyCoffeeAsync(Mapper.MapToCoffeeDto(coffee), clientMoney, typeDeal);

            //Assert
            await _db.DisposeAsync();
            int amountDealActual = deal.Sum(x => x.Denomination * x.CountBanknote);
            Assert.AreEqual(amountDealExpected, amountDealActual);
        }

    }
}
