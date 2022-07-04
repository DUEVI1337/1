
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using CoffeeMachine.Application.Services.Interfaces;
using CoffeeMachine.Domain.Dto;
using CoffeeMachine.Domain.Entities;
using CoffeeMachine.Web.Exceptions;
using CoffeeMachine.Web.Exceptions.CustomExceptions;

using Microsoft.AspNetCore.Mvc;

using Serilog;


namespace CoffeeMachine.Web.Controllers
{
    /// <summary>
    /// work with entity <see cref="Coffee"/> in database.
    /// </summary>
    [Route("coffee/v1")]
    [ApiController]
    public class CoffeeController : Controller
    {
        private readonly ICoffeeService _coffeeService;

        public CoffeeController(ICoffeeService coffeeService)
        {
            _coffeeService = coffeeService;
        }

        /// <summary>
        /// checking possibility of buying coffee
        /// </summary>
        /// <param name="order">order person</param>
        /// <response code="200">return deal</response>
        /// <response code="400">it is impossible to give change</response>
        /// <response code="404">not found coffee</response>
        [HttpPost]
        [Route("BuyCoffee")]
        public async Task<List<BanknoteDto>> BuyCoffee([FromBody] OrderDto order)
        {
            var coffee = await _coffeeService.GetCoffeeDtoByIdAsync(order.CoffeeId);
            if (coffee == null)
                throw new NullReferenceException();

            var deal = await _coffeeService.BuyCoffeeAsync(coffee, order.Banknotes, (TypeDeal)order.TypeDeal);
            if (deal == null)
                throw new NullCashboxException();

            return deal;
        }

        /// <summary>
        /// get coffee by id from database
        /// </summary>
        /// <param name="id">id coffee</param>
        /// <returns><see cref="CoffeeDto"/> or null</returns>
        /// <response code="200">return coffee from database</response>
        /// <response code="404">not found coffee</response>
        [HttpGet]
        [Route("Coffee/{id}")]
        public async Task<CoffeeDto> GetCoffeeDtoById(string id)
        {
            var coffee = await _coffeeService.GetCoffeeDtoByIdAsync(id);
            if (coffee == null)
                throw new NullReferenceException();

            return coffee;
        }

        /// <summary>
        /// get list coffee from database
        /// </summary>
        /// <returns><see cref="List{T}"/> where T <see cref="Coffee"/></returns>
        /// <response code="200">return list of coffee from database</response>
        [HttpGet]
        [Route("MenuCoffee")]
        public async Task<List<CoffeeDto>> GetListCoffeeDto()
        {
            return await _coffeeService.GetListCoffeeDtoAsync();
        }
    }
}