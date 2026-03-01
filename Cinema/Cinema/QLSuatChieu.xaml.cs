using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Cinema
{
    // --- LỚP CONVERTER TÍNH GIỜ KẾT THÚC ---
    public class EndTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is TimeSpan startTime && values[1] is int durationMinutes)
            {
                TimeSpan endTime = startTime.Add(TimeSpan.FromMinutes(durationMinutes));
                return endTime.ToString(@"hh\:mm");
            }
            return "--:--";
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }

    public partial class QLSuatChieu : Page
    {
        DBRapPhimEntities2 db = new DBRapPhimEntities2();

        public QLSuatChieu()
        {
            InitializeComponent();
            LoadDuLieuBieuMau();
            ThucHienLoc();
        }

        private void LoadDuLieuBieuMau()
        {
            try
            {
                // Cấp dữ liệu cho form Thêm mới
                cmb_Phim.ItemsSource = db.phims.ToList();
                cmb_Phim.DisplayMemberPath = "ten_phim";
                cmb_Phim.SelectedValuePath = "ma_phim";

                var dsPhong = db.phongchieux.ToList();
                cmb_Phong.ItemsSource = dsPhong;
                cmb_Phong.DisplayMemberPath = "ten_phong";
                cmb_Phong.SelectedValuePath = "ma_phong";

                // Cấp dữ liệu cho phần Bộ Lọc
                var dsLoc = dsPhong.ToList();
                dsLoc.Insert(0, new phongchieu { ma_phong = 0, ten_phong = "Tất cả rạp" });
                cmb_FilterPhong.ItemsSource = dsLoc;
                cmb_FilterPhong.DisplayMemberPath = "ten_phong";
                cmb_FilterPhong.SelectedValuePath = "ma_phong";
                cmb_FilterPhong.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ThucHienLoc()
        {
            if (dtg_suat_chieu == null) return;

            try
            {
                var query = db.lichchieux.AsQueryable();

                if (cmb_FilterPhong.SelectedValue != null && (int)cmb_FilterPhong.SelectedValue > 0)
                {
                    int maPhongChon = (int)cmb_FilterPhong.SelectedValue;
                    query = query.Where(x => x.ma_phong == maPhongChon);
                }

                if (dp_FilterNgay.SelectedDate.HasValue)
                {
                    DateTime ngayChon = dp_FilterNgay.SelectedDate.Value.Date;
                    query = query.Where(x => x.ngay_chieu == ngayChon);
                }

                string tuKhoa = txt_tim_kiem.Text.Trim().ToLower();
                if (!string.IsNullOrWhiteSpace(tuKhoa) && tuKhoa != "nhập tên phim để tìm...")
                {
                    query = query.Where(x => x.phim != null && x.phim.ten_phim.ToLower().Contains(tuKhoa));
                }

                var ketQua = query.OrderBy(x => x.ngay_chieu).ThenBy(x => x.gio_bat_dau).ToList();
                dtg_suat_chieu.ItemsSource = ketQua;
                txt_ket_qua.Text = $"Hiển thị {ketQua.Count} suất chiếu.";
            }
            catch (Exception) { }
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e) => ThucHienLoc();
        private void txt_tim_kiem_TextChanged(object sender, TextChangedEventArgs e) => ThucHienLoc();

        private void txt_tim_kiem_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_tim_kiem.Text == "Nhập tên phim để tìm...")
            {
                txt_tim_kiem.Text = "";
                txt_tim_kiem.Foreground = Brushes.Black;
            }
        }

        private void txt_tim_kiem_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_tim_kiem.Text))
            {
                txt_tim_kiem.Text = "Nhập tên phim để tìm...";
                txt_tim_kiem.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A5A6"));
                ThucHienLoc();
            }
        }

        private void btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            cmb_FilterPhong.SelectedIndex = 0;
            dp_FilterNgay.SelectedDate = null;
            txt_tim_kiem.Text = "Nhập tên phim để tìm...";
            txt_tim_kiem.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A5A6"));
            ThucHienLoc();
        }

        private void Modal_Phong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_Phong.SelectedItem is phongchieu selectedPhong && txt_ThongBaoBaoTri != null)
            {
                txt_ThongBaoBaoTri.Visibility = (selectedPhong.tinh_trang == "BaoTri") ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void btn_hien_form_them_Click(object sender, RoutedEventArgs e)
        {
            // Reset form trống trước khi hiện
            cmb_Phim.SelectedIndex = -1;
            cmb_Phong.SelectedIndex = -1;
            dp_NgayChieu.SelectedDate = null;
            txt_GioChieu.Text = "";
            txt_GiaVe.Text = "";
            txt_ThongBaoBaoTri.Visibility = Visibility.Collapsed;

            ModalOverlay.Visibility = Visibility.Visible;
        }

        private void btn_HuyNhap_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
        }

        private void btn_LuuNhanh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmb_Phim.SelectedValue == null || cmb_Phong.SelectedValue == null || dp_NgayChieu.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(txt_GioChieu.Text) || string.IsNullOrWhiteSpace(txt_GiaVe.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                lichchieu lcMoi = new lichchieu
                {
                    ma_phim = (int)cmb_Phim.SelectedValue,
                    ma_phong = (int)cmb_Phong.SelectedValue,
                    ngay_chieu = dp_NgayChieu.SelectedDate.Value,
                    gio_bat_dau = TimeSpan.Parse(txt_GioChieu.Text),
                    gia_ve_co_ban = decimal.Parse(txt_GiaVe.Text),
                    nguoi_lap_lich = 1
                };

                db.lichchieux.Add(lcMoi);
                db.SaveChanges();

                MessageBox.Show("Thêm suất chiếu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                ThucHienLoc();
                ModalOverlay.Visibility = Visibility.Collapsed;
            }
            catch (FormatException)
            {
                MessageBox.Show("Giờ (VD: 19:30) hoặc Giá vé sai định dạng!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_xoa_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is lichchieu suatChieuCanXoa)
            {
                MessageBoxResult result = MessageBox.Show($"Xóa suất chiếu '{suatChieuCanXoa.phim.ten_phim}' ngày {suatChieuCanXoa.ngay_chieu:dd/MM/yyyy}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.lichchieux.Remove(suatChieuCanXoa);
                        db.SaveChanges();
                        ThucHienLoc();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void FastEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) btn_LuuNhanh_Click(sender, e);
        }
    }
}