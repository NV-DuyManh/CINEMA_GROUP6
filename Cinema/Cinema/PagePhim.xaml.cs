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
using System.Data;
using System.Data.SqlClient;
namespace Cinema
{
    /// <summary>
    /// Interaction logic for PagePhim.xaml
    /// </summary>
    public partial class PagePhim : Page
    {
        public PagePhim()
        {
            InitializeComponent();
            LoadDuLieuPhim();
        }

        private void LoadDuLieuPhim()
        {
            try
            {
                using (DBRapPhimEntities2 db = new DBRapPhimEntities2())
                {
                    dgPhim.ItemsSource = db.phims
                                           .OrderBy(p => p.ma_phim)
                                           .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu phim:\n" + ex.Message);
            }
        }

        private void BtnThemMoi_Click(object sender, RoutedEventArgs e)
        {
            WindowThemPhim w = new WindowThemPhim();

            if (w.ShowDialog() == true)
            {
                LoadDuLieuPhim();
            }
        }

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgPhim.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn phim cần sửa!");
                return;
            }

            phim selected = (phim)dgPhim.SelectedItem;

            WindowThemPhim win = new WindowThemPhim();

            win.isEdit = true;
            win.MaPhim = selected.ma_phim;

            win.txtTenPhim.Text = selected.ten_phim;
            win.txtMaTheLoai.Text = selected.ma_the_loai.ToString();
            win.txtThoiLuong.Text = selected.thoi_luong.ToString();
            win.dpNgayKhoiChieu.SelectedDate = selected.ngay_khoi_chieu;
            win.dpNgayKetThuc.SelectedDate = selected.ngay_ket_thuc;

            win.txtMoTa.Text = selected.mo_ta;
            if (selected.trang_thai != null)
            {
                foreach (ComboBoxItem item in win.cbTrangThai.Items)
                {
                    if (item.Content.ToString() == selected.trang_thai)
                    {
                        win.cbTrangThai.SelectedItem = item;
                        break;
                    }
                }
            }

            if (selected.nguoi_nhap != null)
                win.txtNguoiNhap.Text = selected.nguoi_nhap.ToString();

            if (win.ShowDialog() == true)
            {
                LoadDuLieuPhim();
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgPhim.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn phim cần xóa!");
                return;
            }

            phim selected = (phim)dgPhim.SelectedItem;

            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa phim này?",
                "Xác nhận",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (DBRapPhimEntities2 db = new DBRapPhimEntities2())
                    {
                        var p = db.phims.Find(selected.ma_phim);
                        if (p != null)
                        {
                            db.phims.Remove(p);
                            db.SaveChanges();
                        }
                    }

                    MessageBox.Show("Xóa thành công!");
                    LoadDuLieuPhim();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void dgPhim_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
