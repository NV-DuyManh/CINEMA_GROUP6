using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cinema
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }
    public static class UserSession
    {
        public static string TaiKhoan { get; set; }
        public static string ChucVu { get; set; }
        public static string HoTen { get; set; }
    }
}
