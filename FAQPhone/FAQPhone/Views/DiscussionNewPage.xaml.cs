﻿using FAQPhone.Infarstructure;
using FAQPhone.Infrastructure;
using FAQPhone.Models;
using FAQPhone.Models.Interfaces;
using FAQPhone.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FAQPhone.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiscussionNewPage : ContentPage
    {
        public DiscussionNewPage(IDiscussionCreator discussionCreator, DiscountModel discount, int pushCount)
        {
            InitializeComponent();
            this.Appearing += (sender, e) => {
                for (var i = 0; i < pushCount; i++)
                {
                    this.Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                }
            };            
            var factory = App.Resolve<DiscussionEditViewModelFactory>();
            BindingContext = factory.Create(this, discussionCreator, discount);
        }
    }

    public class DiscussionEditViewModelFactory
    {
        public DiscussionEditViewModelFactory()
        {
            
        }
        public DiscussionEditViewModel Create(ContentPage page, IDiscussionCreator discussionCreator, DiscountModel discount)
        {
            return new DiscussionEditViewModel(page, discussionCreator, discount);
        }
    }

    public class DiscussionEditViewModel : BaseViewModel
    {
        public DiscussionEditViewModel(ContentPage page, IDiscussionCreator discussionCreator, DiscountModel discount) : base(page)
        {            
            this.CanNext = false;
            this.UsedDiscount = false;
            this.HasDiscount = (discount != null);
            if (this.HasDiscount)
            {
                this.discount = discount;
                this.discountPrice = discount.price + " " +
                    ResourceManagerHelper.GetValue("unit_of_mony_caption");                
            }
            this.NextCommand = new Command(async () => await nextCommand());
            this.DiscountCommand = new Command(async () => await discountCommand());
            this.discussionCreator = discussionCreator;
            this.price = 
                ResourceManagerHelper.GetValue("discussion_recive_price") + ":" +
                (discussionCreator?.price ?? 0) + " " + 
                ResourceManagerHelper.GetValue("unit_of_mony_caption");
        }

        string _title;
        public string title
        {
            get { return _title; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length <= 30)
                {
                    _title = value;
                    CanNext = !string.IsNullOrWhiteSpace(_title);
                }
                OnPropertyChanged();
            }
        }

        bool _CanNext;
        public bool CanNext
        {
            get { return _CanNext; }
            set { _CanNext = value; OnPropertyChanged(); }
        }

        bool _HasDiscount;
        public bool HasDiscount
        {
            get { return _HasDiscount; }
            set { _HasDiscount = value; OnPropertyChanged(); }
        }

        bool _UsedDiscount;
        public bool UsedDiscount
        {
            get { return _UsedDiscount; }
            set { _UsedDiscount = value; OnPropertyChanged(); }
        }

        string _price;
        public string price
        {
            get { return _price; }
            set { _price = value; OnPropertyChanged(); }
        }

        string _discountPrice;
        public string discountPrice
        {
            get { return _discountPrice; }
            set { _discountPrice = value; OnPropertyChanged(); }
        }

        private IDiscussionCreator discussionCreator { get; set; }
        private DiscountModel discount { get;set; }

        public ICommand DiscountCommand { protected set; get; }

        public async Task discountCommand()
        {
            this.UsedDiscount = !this.UsedDiscount;
        }
        public ICommand NextCommand { protected set; get; }

        public async Task nextCommand()
        {
            /////
            DiscussionModel model = new DiscussionModel()
            {
                title = this.title,
                from = new AccountModel() { username = App.Username },
                createDate = DateTime.Now,
                state = Constants.DISCUSSION_STATE_CREATE,
                userRead = false,
                operatorRead = false,
                usedDiscount = new DiscountModel() { _id = this.discount._id },
                department = new DepartmentModel() { _id = this.discussionCreator._id },
                items = new DiscussionDetailModel[] { }
            };
            await this.Navigation.PushAsync(new ChatPage(Constants.USER_INPROGRESS_FAQ, model, 1));
            //await this.Navigation.PushAsync(new ChatPage(Constants.USER_INPROGRESS_FAQ, model));
        }
    }
}
