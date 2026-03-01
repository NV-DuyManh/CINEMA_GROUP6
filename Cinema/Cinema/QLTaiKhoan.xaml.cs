using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cinema
{
    public partial class QLTaiKhoan : Page
    {
        DBRapPhimEntities2 db = new DBRapPhimEntities2();

        public QLTaiKhoan()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            dgNguoiDung.ItemsSource = db.nguoidungs.ToList();
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new DBRapPhimEntities2())
                {
                    var moi = new nguoidung
                    {
                        tai_khoan = txtTaiKhoan.Text,
                        mat_khau = txtMatKhau.Text,
                        ho_ten = txtHoTen.Text,
                        chuc_vu = (cbChucVu.SelectedItem as ComboBoxItem)?.Content.ToString()
                    };

                    db.nguoidungs.Add(moi);
                    db.SaveChanges();

                    MessageBox.Show("Thêm thành công!");
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message;
                MessageBox.Show("Lỗi chi tiết: " + message);
            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgNguoiDung.SelectedItem as nguoidung;
            if (selected != null)
            {
                var user = db.nguoidungs.Find(selected.ma_nguoi_dung);
                user.tai_khoan = txtTaiKhoan.Text;
                if (!string.IsNullOrEmpty(txtMatKhau.Text))
                    user.mat_khau = txtMatKhau.Text;
                user.ho_ten = txtHoTen.Text;
                user.chuc_vu = (cbChucVu.SelectedItem as ComboBoxItem)?.Content.ToString();

                db.SaveChanges();
                LoadData();
                MessageBox.Show("Cập nhật thành công!");
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgNguoiDung.SelectedItem as nguoidung;

            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xóa!", "Thông báo");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa tài khoản {selected.tai_khoan} không?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var userInDb = db.nguoidungs.SingleOrDefault(u => u.ma_nguoi_dung == selected.ma_nguoi_dung);

                    if (userInDb != null)
                    {
                        db.nguoidungs.Remove(userInDb);
                        db.SaveChanges();

                        MessageBox.Show("Đã xóa dữ liệu thành công!", "Thông báo");

                        LoadData();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy dữ liệu trong hệ thống.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xóa tài khoản này vì có dữ liệu liên quan! \nChi tiết: " + ex.Message, "Lỗi");
                }
            }
        }

        private void dgNguoiDung_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = dgNguoiDung.SelectedItem as nguoidung;
            if (selected != null)
            {
                txtTaiKhoan.Text = selected.tai_khoan;
                txtHoTen.Text = selected.ho_ten;
                txtMatKhau.Text = selected.mat_khau;

                foreach (ComboBoxItem item in cbChucVu.Items)
                {
                    if (item.Content.ToString() == selected.chuc_vu)
                    {
                        cbChucVu.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        void ClearFields()
        {
            txtTaiKhoan.Clear();
            txtMatKhau.Clear();
            txtHoTen.Clear();
            cbChucVu.SelectedIndex = -1;
        }

        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            LoadData();
        }
    }
}