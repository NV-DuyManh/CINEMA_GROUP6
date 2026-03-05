using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace Cinema
{
    public partial class MainWindow : Window
    {
        string strCon = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DBRapPhim.mdf;Integrated Security=True;Connect Timeout=30";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => txtTaiKhoan.Focus();
        }

        private void txtTaiKhoan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtMatKhau.Focus();
            }
        }

        private void txtMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnDangNhap_Click(sender, new RoutedEventArgs());
            }
        }

        private void BtnDangNhap_Click(object sender, RoutedEventArgs e)
        {
            string taiKhoan = txtTaiKhoan.Text.Trim();
            string matKhau = txtMatKhau.Password.Trim();

            if (string.IsNullOrEmpty(taiKhoan) || string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTaiKhoan.Focus();
                return;
            }

            try
            {
                using (SqlConnection sqlCon = new SqlConnection(strCon))
                {
                    sqlCon.Open();
                    string query = "SELECT tai_khoan, ho_ten, chuc_vu FROM nguoidung WHERE tai_khoan = @taikhoan AND mat_khau = @matkhau";

                    using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                    {
                        cmd.Parameters.AddWithValue("@taikhoan", taiKhoan);
                        cmd.Parameters.AddWithValue("@matkhau", matKhau);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserSession.TaiKhoan = reader["tai_khoan"].ToString();
                                UserSession.HoTen = reader["ho_ten"].ToString();
                                UserSession.ChucVu = reader["chuc_vu"].ToString();

                                MessageBox.Show($"Chào mừng {UserSession.HoTen} ({UserSession.ChucVu}) trở lại!", "Thông báo");

                                Menu formMenu = new Menu();
                                formMenu.Show();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                txtMatKhau.Clear();
                                txtTaiKhoan.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        private void txtQuenMatKhau_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Vui lòng liên hệ Admin để được cấp lại mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
