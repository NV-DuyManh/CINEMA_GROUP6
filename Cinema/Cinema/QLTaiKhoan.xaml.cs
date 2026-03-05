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
            // Làm mới context để lấy dữ liệu mới nhất từ DB
            db = new DBRapPhimEntities2();
            dgNguoiDung.ItemsSource = db.nguoidungs.ToList();
        }

        // Hàm kiểm tra nhập liệu dùng chung cho cả Thêm và Sửa
        bool IsInputValid()
        {
            if (string.IsNullOrWhiteSpace(txtTaiKhoan.Text))
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTaiKhoan.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMatKhau.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên nhân viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtHoTen.Focus();
                return false;
            }
            if (cbChucVu.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn chức vụ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbChucVu.IsDropDownOpen = true;
                return false;
            }
            return true;
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInputValid()) return; // Dừng nếu để trống

            try
            {
                var moi = new nguoidung
                {
                    tai_khoan = txtTaiKhoan.Text.Trim(),
                    mat_khau = txtMatKhau.Text,
                    ho_ten = txtHoTen.Text.Trim(),
                    chuc_vu = (cbChucVu.SelectedItem as ComboBoxItem)?.Content.ToString()
                };

                db.nguoidungs.Add(moi);
                db.SaveChanges();

                MessageBox.Show("Thêm tài khoản mới thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm: " + ex.Message);
            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgNguoiDung.SelectedItem as nguoidung;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản từ danh sách để cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!IsInputValid()) return; // Dừng nếu để trống khi sửa

            try
            {
                var user = db.nguoidungs.Find(selected.ma_nguoi_dung);
                if (user != null)
                {
                    user.tai_khoan = txtTaiKhoan.Text.Trim();
                    user.mat_khau = txtMatKhau.Text;
                    user.ho_ten = txtHoTen.Text.Trim();
                    user.chuc_vu = (cbChucVu.SelectedItem as ComboBoxItem)?.Content.ToString();

                    db.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgNguoiDung.SelectedItem as nguoidung;

            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa tài khoản [{selected.tai_khoan}] không?\nHành động này không thể hoàn tác.",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var userInDb = db.nguoidungs.Find(selected.ma_nguoi_dung);
                    if (userInDb != null)
                    {
                        db.nguoidungs.Remove(userInDb);
                        db.SaveChanges();

                        MessageBox.Show("Đã xóa dữ liệu thành công!", "Thông báo");
                        LoadData();
                        ClearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xóa tài khoản này vì có dữ liệu liên quan (lịch trực, hóa đơn...)!", "Lỗi hệ thống");
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

                // Tìm và chọn Item trong ComboBox khớp với dữ liệu
                foreach (ComboBoxItem item in cbChucVu.Items)
                {
                    if (item.Content.ToString().Contains(selected.chuc_vu))
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
            dgNguoiDung.SelectedItem = null;
        }

        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            LoadData();
        }
    }
}