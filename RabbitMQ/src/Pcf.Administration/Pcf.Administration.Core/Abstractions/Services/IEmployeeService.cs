﻿using System;
using System.Threading.Tasks;

namespace Pcf.Administration.Core.Abstractions.Services
{
    public interface IEmployeeService
    {
        Task AppliedPromocodesAsync(Guid id);
    }
}
