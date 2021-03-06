﻿using FAQPhone.Helpers;
using FAQPhone.Infarstructure;
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
    public partial class SendCodePage : ContentPage
    {
        public SendCodePage()
        {
            InitializeComponent();
            var factory = App.Resolve<SendCodeViewModelFactory>();
            BindingContext = factory.Create(this);
        }
    }
    
    public class SendCodeViewModelFactory
    {
        IAccountService accountService;
        public SendCodeViewModelFactory(IAccountService accountService)
        {
            this.accountService = accountService;
        }
        public SendCodeViewModel Create(ContentPage page)
        {
            return new SendCodeViewModel(this.accountService, page);
        }
    }
    
    public class SendCodeViewModel : BaseViewModel
    {
        public SendCodeViewModel(IAccountService accountService, ContentPage page) : base(page)
        {
            this.accountService = accountService;
            this.SendCodeCommand = new Command(async () => await sendCodeCommand());
            var languageList = new string[] { "Fa", "Ar" };
            this.LanguageList = new ObservableCollection<AttributeModel>();
            foreach (var item in languageList)
            {
                string key = string.Format("setting_language_{0}", item.ToLower());
                string val = ResourceManagerHelper.GetValue(key);
                this.LanguageList.Add(new AttributeModel() { caption = val, _id = key });
            }
        }
        private IAccountService accountService { get; set; }

        private ObservableCollection<AttributeModel> _LanguageList;
        public ObservableCollection<AttributeModel> LanguageList
        {
            get { return _LanguageList; }
            set { _LanguageList = value; }
        }

        private AttributeModel _SelectedLanguage;
        public AttributeModel SelectedLanguage
        {
            get { return _SelectedLanguage; }
            set
            {
                _SelectedLanguage = value;
                OnPropertyChanged();
            }
        }

        string _mobile;
        public string mobile
        {
            get { return _mobile; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length <= 11)
                {
                    _mobile = value;                    
                }
                OnPropertyChanged();
            }
        }
        public ICommand SendCodeCommand { protected set; get; }

        public async Task sendCodeCommand()
        {
            /////
            if (!string.IsNullOrEmpty(this.SelectedLanguage?._id))
            {
                Settings.Language = this.SelectedLanguage._id;
            }
            var mobile = this.mobile.ToEnglishNumber();
            var codeResult = await this.accountService.SendCode(mobile);            
            await this.Navigation.PushAsync(new SecurityCodePage(mobile, codeResult));
        }
    }
}
