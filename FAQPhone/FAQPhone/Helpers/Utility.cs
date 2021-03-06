﻿using FAQPhone.Infarstructure;
using FAQPhone.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FAQPhone.Helpers
{
    public class Utility
    {
        public static readonly string[] monthNames = { 
            "فروردین", "اردیبهشت", "خرداد", 
            "تیر", "مرداد", "شهریور", 
            "مهر", "آبان", "آذر", 
            "دی", "بهمن", "اسفند"
        };
    public static string GetImage(string sourceImage) 
        {
            string imagePath;
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    imagePath = sourceImage;
                    break;
                case Device.iOS:
                    imagePath = sourceImage + ".png";
                    break;
                case Device.WinPhone:
                    imagePath = "Images/" + sourceImage + ".png";
                    break;
                case Device.Windows:
                    imagePath = "Images/" + sourceImage + ".png";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return imagePath;

        }
        public static Task RegulareAlert(string message)
        {
            string title = Constants.MESSAGE_TITLE_ALERT;
            string cancel = Constants.COMMAND_OK;
            var t = ResourceManagerHelper.GetValue(title);
            var m = message ?? ResourceManagerHelper.GetValue(Constants.MESSAGE_UNKNOWN_ERROR);
            var c = ResourceManagerHelper.GetValue(cancel);
            return Application.Current.MainPage.DisplayAlert(t, m, c);
        }
        public static Task<bool> RegulareConfirm(string message)
        {
            string title = Constants.MESSAGE_TITLE_ALERT;
            string accept = Constants.COMMAND_YES;
            string cancel = Constants.COMMAND_NO;
            var t = ResourceManagerHelper.GetValue(title);
            var m = message ?? ResourceManagerHelper.GetValue(Constants.MESSAGE_UNKNOWN_ERROR);
            var a = ResourceManagerHelper.GetValue(accept);
            var c = ResourceManagerHelper.GetValue(cancel);
            return Application.Current.MainPage.DisplayAlert(t, m, a, c);
        }
        public static Task Alert(string message = Constants.MESSAGE_UNKNOWN_ERROR, string title = Constants.MESSAGE_TITLE_ALERT, string cancel = Constants.COMMAND_OK)
        {
            var t = ResourceManagerHelper.GetValue(title);
            var m = ResourceManagerHelper.GetValue(message) ?? ResourceManagerHelper.GetValue(Constants.MESSAGE_UNKNOWN_ERROR);
            var c = ResourceManagerHelper.GetValue(cancel);
            return Application.Current.MainPage.DisplayAlert(t, m, c);
        }
        public static Task<bool> Confirm(string message = Constants.MESSAGE_ARE_YOU_SURE, string title = Constants.MESSAGE_TITLE_ALERT, string accept = Constants.COMMAND_YES, string cancel = Constants.COMMAND_NO)
        {
            var t = ResourceManagerHelper.GetValue(title);
            var m = ResourceManagerHelper.GetValue(message) ?? ResourceManagerHelper.GetValue(Constants.MESSAGE_UNKNOWN_ERROR);
            var a = ResourceManagerHelper.GetValue(accept);
            var c = ResourceManagerHelper.GetValue(cancel);
            return Application.Current.MainPage.DisplayAlert(t, m, a, c);
        }

        public static int[] GetVersionInfo(string version)
        {
            var arr = version.Split('.');
            var info = new int[] { 0, 0, 0 };
            for (var i = 0; i < 3; i++)
            {
                if (arr.Length > i)
                {
                    var v = arr[i];
                    int vr;
                    if (int.TryParse(v, out vr))
                    {
                        info[i] = vr;
                    }
                }
            }
            return info;
        }

        public static int CompareVersion()
        {
            var app_version = ResourceManagerHelper.GetValue(Constants.APP_VERSION);
            var arr1 = GetVersionInfo(app_version);
            var arr2 = GetVersionInfo(App.SuportVersion);
            var result = 0;
            for (int i = 0; i < 3; i++)
            {
                if (arr1[i] < arr2[i])
                {
                    result = -1;
                }
                else if (arr1[i] > arr2[i])
                {
                    result = 1;
                }
                if (result != 0)
                    break;
            }
            return result;
        }

        public static int[] MiladiToShamsi(DateTime dt)
        {
            var pCalendar = DependencyService.Get<CalendarService.IPersianCalendarService>();
            var pc = pCalendar.GetCalendar();
            var y = pc.GetYear(dt);
            var m = pc.GetMonth(dt);
            var d = pc.GetDayOfMonth(dt);
            int[] arr = new int[] { y, m, d };
            return arr;
        }

        public static string MiladiToShamsiString(DateTime dt)
        {
            var pCalendar = DependencyService.Get<CalendarService.IPersianCalendarService>();
            var pc = pCalendar.GetCalendar();
            var y = pc.GetYear(dt);
            var m = pc.GetMonth(dt);
            var d = pc.GetDayOfMonth(dt);
            return string.Format("{0}/{1}/{2}", y, m, d );
        }

        public static string MiladiToShamsiAndTime(DateTime dt)
        {
            var pCalendar = DependencyService.Get<CalendarService.IPersianCalendarService>();
            var pc = pCalendar.GetCalendar();
            var y = pc.GetYear(dt);
            var M = pc.GetMonth(dt);
            var d = pc.GetDayOfMonth(dt);
            var h = dt.Hour;
            var m = dt.Minute;
            var Mn = monthNames[M - 1];
            var n = DateTime.Now;
            var ny = pc.GetYear(n);
            var result = (n.Date.CompareTo(dt.Date) == 0)
                ? string.Format("{0}:{1}", h, m)
                : ((ny == y) 
                    ? string.Format("{0} {1} {2}:{3}", d, Mn, h, m) 
                    : string.Format("{0} {1} {2} {3}:{4}", d, Mn, y, h, m));
            return result;
        }

        public static DateTime ShamsiToMiladi(int y, int m, int d)
        {
            var pCalendar = DependencyService.Get<CalendarService.IPersianCalendarService>();
            var pc = pCalendar.GetCalendar();
            return pc.ToDateTime(y, m, d, 0, 0, 0, 0);
        }
    }
}
