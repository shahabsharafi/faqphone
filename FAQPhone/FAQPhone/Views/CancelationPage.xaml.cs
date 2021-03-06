﻿using FAQPhone.Infarstructure;
using FAQPhone.Infrastructure;
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
    public partial class CancelationPage : ContentPage
    {
        public CancelationPage(DiscussionModel model)
        {
            InitializeComponent();
            var factory = App.Resolve<CancelationViewModelFactory>();
            BindingContext = factory.Create(this, model);
        }
    }

    public class CancelationViewModelFactory
    {
        IDiscussionService discussionService;
        public CancelationViewModelFactory(IDiscussionService discussionService)
        {
            this.discussionService = discussionService;
        }
        public CancelationViewModel Create(ContentPage page, DiscussionModel model)
        {
            return new CancelationViewModel(this.discussionService, page, model);
        }
    }

    public class CancelationViewModel : BaseViewModel
    {

        public CancelationViewModel(IDiscussionService discussionService, ContentPage page, DiscussionModel model) : base(page)
        {
            this.model = model;
            this.discussionService = discussionService;
            this.SelectItemCommand = new Command<MenuItemModel>(async (item) => await selectItemCommand(item));
            this.List = new ObservableCollection<MenuItemModel>();
            List<MenuItemModel> items = new List<MenuItemModel>();
            items.Add(new MenuItemModel() { CommandName = Constants.CANCELATION_UNCLEAR });
            items.Add(new MenuItemModel() { CommandName = Constants.CANCELATION_UNRELATED });
            items.Add(new MenuItemModel() { CommandName = Constants.CANCELATION_ANNOYING });
            items.Add(new MenuItemModel() { CommandName = Constants.CANCELATION_OFFENSIV });
            this.setList(items);
        }
        private IDiscussionService discussionService { get; set; }
        DiscussionModel model { get; set; }

        ObservableCollection<MenuItemModel> _list;
        public ObservableCollection<MenuItemModel> List
        {
            get { return _list; }
            set { _list = value; OnPropertyChanged(); }
        }

        private void setList(List<MenuItemModel> list)
        {
            this.List.Clear();
            foreach (var item in list)
            {
                this.List.Add(item);
            }
        }

        object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        public ICommand SelectItemCommand { protected set; get; }

        public async Task selectItemCommand(MenuItemModel item)
        {
            if (item == null)
                return;
            if (!string.IsNullOrEmpty(item.CommandName))
            {
                string message = "";
                int cancelation = 0;
                switch (item.CommandName)
                {
                    case Constants.CANCELATION_UNCLEAR:
                        message = Constants.CANCELATION_UNCLEAR_TEXT;
                        cancelation = 1;
                        break;
                    case Constants.CANCELATION_UNRELATED:
                        message = Constants.CANCELATION_UNRELATED_TEXT;
                        cancelation = 2;
                        break;
                    case Constants.CANCELATION_ANNOYING:
                        message = Constants.CANCELATION_ANNOYING_TEXT;
                        cancelation = 3;
                        break;
                    case Constants.CANCELATION_OFFENSIV:
                        message = Constants.CANCELATION_OFFENSIV_TEXT;
                        cancelation = 4;
                        break;
                }
                var l = model.items.ToList();
                l.Add(new DiscussionDetailModel()
                {
                    createDate = DateTime.Now,
                    owner = new AccountModel() { username = App.Username },
                    text = ResourceManagerHelper.GetValue(message)
                });
                model.items = l.ToArray();
                model.state = Constants.DISCUSSION_STATE_REPORT;
                model.cancelation = cancelation;
                model.userRead = false;
                model.to = new AccountModel() { username = App.Username };
                await this.discussionService.Save(model);
                this.SelectedItem = null;
                this.Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                await this.Navigation.PopAsync();
            }
        }
    }
}