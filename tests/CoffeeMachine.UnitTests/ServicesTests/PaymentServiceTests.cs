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
    public class PaymentServiceTests
    {
        private PaymentService _paymentService;
        private DataContext _db;

        [SetUp]
        public void Setup()
        {
            var _dbOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _db = new DataContext(_dbOptions);
            var uow = new UnitOfWork(_db, new CoffeeRepository(_db), new BalanceRepository(_db),
                new BanknoteCashboxRepository(_db), new PaymentRepository(_db), new IncomeRepository(_db));
            _paymentService = new PaymentService(uow);
        }

        [Test]
        public async Task AddPaymentAsync_CheckInDb_AddNewPaymentInDb()
        {
            //Arrange
            const int clientMoney = 100;
            const int deal = 50;
            Payment paymentExpected = new ()
            {
                PaymentId = Guid.NewGuid(), CoffeeId = Guid.NewGuid(), ClientMoney = clientMoney, Deal = deal,
            };
            _db.Payments.Add(paymentExpected);
            await _db.SaveChangesAsync();
            
            //Act
            _paymentService.AddPayment(clientMoney, paymentExpected.CoffeeId.ToString(), deal);

            //Assert
            var paymentActual = await _db.Payments.FindAsync(paymentExpected.PaymentId);
            await _db.DisposeAsync();
            paymentActual.Should().BeEquivalentTo(paymentExpected);
        }
    }
}
