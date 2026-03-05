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
            if (UserSession.ChucVu != "Admin")
            {
                btnQLTaiKhoan.Visibility = Visibility.Collapsed;
            }
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
            
            if (UserSession.ChucVu == "Admin")
            {
                MainFrame.Navigate(new QLTaiKhoan());
            }
            else
            {
                MessageBox.Show("Bạn không có quyền truy cập vào chức năng Quản lý tài khoản. Chỉ dành cho Quản trị viên!",
                                "Truy cập bị từ chối",
                                MessageBoxButton.OK,
                                MessageBoxImage.Stop);
            }
        }

        private void BtnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}