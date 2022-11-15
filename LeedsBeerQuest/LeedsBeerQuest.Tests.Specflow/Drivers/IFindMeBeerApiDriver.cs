﻿using LeedsBeerQuest.Api.Models;

namespace LeedsBeerQuest.Tests.Specflow.Drivers
{
    internal interface IFindMeBeerApiDriver
    {
        BeerEstablishmentLocation[] Establishments { get; }

        Task<bool> GetBeerEstablishments();
        Task<bool> ImportData();
    }
}