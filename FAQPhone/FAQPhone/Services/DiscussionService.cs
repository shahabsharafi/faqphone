﻿using FAQPhone.Inferstructure;
using FAQPhone.Models;
using FAQPhone.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAQPhone.Services
{
    public class DiscussionService:  RestService<DiscussionModel>, IDiscussionService
    {
        public DiscussionService() : base()
        {
            this._relativeUrl = "discussions/{0}";
        }

        public async Task<List<DiscussionModel>> GetList(bool isUser, int[] states)
        {
            string url = string.Format(Constants.RestUrl, string.Format("discussions/getlist/{0}/{1}/{2}", isUser, App.Bag.username, string.Join(",", states)));
            var data = await this.get<List<DiscussionModel>>(url);
            return data;
        }

        public async Task<DiscussionModel> Recive()
        {
            string url = string.Format(Constants.RestUrl, "discussions/recive");
            var data = await this.get<DiscussionModel>(url);
            return data;
        }
    }
}
