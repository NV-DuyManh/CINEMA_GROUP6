using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Cần thiết cho MouseButtonEventArgs

namespace Cinema
{
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        // Sự kiện click vào tiêu đề để quay về trang chào mừng
        private void TxtTieuDe_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new WelcomePage());
        }

        private void BtnPhim_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PagePhim());
        }

        private void BtnSanPham_Click(object sender, RoutedEventArgs e)
        {
           MainFrame.Navigate(new qlsp());
        }

        private void BtnSuatChieu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new QLSuatChieu());
        }

        private void BtnTaiKhoan_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new QLTaiKhoan());
        }

        private void BtnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}