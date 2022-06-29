﻿using System.Collections.Generic;
using System.Threading.Tasks;

using CoffeeMachine.Domain.Dto;
using CoffeeMachine.Domain.Entities;

namespace CoffeeMachine.Application.Service.Interfaces
{
    /// <summary>
    /// Work with <see cref="Balance"/> entity from 'Infrastructure' layer
    /// </summary>
    public interface IBalanceService
    {
        /// <summary>
        /// get all <see cref="Balance"/> in database that converts to list <see cref="BalanceDto"/>
        /// </summary>
        /// <returns><see cref="List{T}"/> where T <see cref="BalanceDto"/></returns>
        Task<List<BalanceDto>> GetBalancesDtoAsync();
    }
}