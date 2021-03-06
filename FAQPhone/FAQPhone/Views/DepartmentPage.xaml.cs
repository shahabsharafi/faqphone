﻿using FAQPhone.Helpers;
using FAQPhone.Infarstructure;
using FAQPhone.Models;
using FAQPhone.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class DepartmentPage : ContentPage
    {
        public DepartmentPage(List<DepartmentModel> list = null, int pushCount = 1)
        {
            InitializeComponent();
            var factory = App.Resolve<DepartmentPageViewModelFactory>();
            BindingContext = factory.Create(this, list, pushCount);
        }
    }

    public class DepartmentPageViewModelFactory
    {
        IDepartmentService departmentService;
        IDiscountService discountService;
        IAccountService accountService;
        public DepartmentPageViewModelFactory(IDepartmentService departmentService, IDiscountService discountService, IAccountService accountService)
        {
            this.departmentService = departmentService;
            this.discountService = discountService;
            this.accountService = accountService;
        }
        public DepartmentPageViewModel Create(ContentPage page, List<DepartmentModel> list, int pushCount)
        {
            return new DepartmentPageViewModel(this.departmentService, this.discountService, this.accountService, page, list, pushCount);
        }
    }

    public class DepartmentPageViewModel : BaseViewModel
    {

        public DepartmentPageViewModel(IDepartmentService departmentService, IDiscountService discountService, IAccountService accountService, ContentPage page, List<DepartmentModel> list, int pushCount) : base(page)
        {
            this._pushCount = pushCount;
            this.departmentService = departmentService;
            this.discountService = discountService;
            this.accountService = accountService;
            this.SelectItemCommand = new Command<DepartmentModel>(async (model) => await selectItemCommand(model));
            this.List = new ObservableCollection<DepartmentModel>();
            if (list == null)
            {
                Task.Run(async () => await loadItems(null));
            }
            else
            {
                this.setList(list);
            }
        }
        private IDepartmentService departmentService { get; set; }
        private IDiscountService discountService { get; set; }
        private IAccountService accountService { get; set; }

        int _pushCount;

        object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        ObservableCollection<DepartmentModel> _list;
        public ObservableCollection<DepartmentModel> List
        {
            get { return _list; }
            set { _list = value; OnPropertyChanged(); }
        }

        public async Task loadItems(DepartmentModel model)
        {
            var list = await this.departmentService.GetByParent(model._id);
            if (model == null)
            {
                this.setList(list);
            }
            else if (list == null || list.Count == 0)
            {
                if (App.Blocked)
                {
                    await Utility.Alert("message_user_blocked");
                }
                else
                {
                    var discount = await this.discountService.Select(model._id);
                    await this.Navigation.PushAsync(new DiscussionNewPage(model, null, discount, this._pushCount));
                }
            }
            else
            {
                await this.Navigation.PushAsync(new DepartmentPage(list, (this._pushCount + 1)));
            }
        }

        private void setList(List<DepartmentModel> list)
        {
            this.List.Clear();
            foreach (var item in list)
            {
                this.List.Add(item);
            }
        }

        public ICommand SelectItemCommand { protected set; get; }

        public async Task selectItemCommand(DepartmentModel model)
        {
            if (model == null)
                return;
            var flag = true;
            if (model.requireAttribute)
            {
                flag = await this.CheckRequireAttribute();               
            }
            if (flag)
            {
                await loadItems(model);
            }
            else
            {
                await Utility.Alert("message_profile_not_completed");
            }
            this.SelectedItem = null;
        }

        bool? _completeProfile = null;
        private async Task<bool> CheckRequireAttribute()
        {
            if (!this._completeProfile.HasValue)
            {
                AccountModel me = await this.accountService.GetMe();
                this._completeProfile =
                    (!string.IsNullOrWhiteSpace(me?.profile?.sect)) &&
                    (!string.IsNullOrWhiteSpace(me?.profile?.reference));
            }
            return this._completeProfile.Value;
        }
    }
}