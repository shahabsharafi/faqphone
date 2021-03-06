﻿using FAQPhone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAQPhone.Services.Interfaces
{
    public interface IDiscountService: IRestService<DiscountModel>
    {
        Task<List<DiscountModel>> GetList();
        Task<DiscountModel> Select(string departmentId);
    }
}
