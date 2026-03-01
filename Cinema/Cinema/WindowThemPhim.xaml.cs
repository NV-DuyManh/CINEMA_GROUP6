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
using System.Windows.Shapes;
using System.Data.SqlClient;
using Cinema;        // nếu cần
using System.Data.Entity;
namespace Cinema
{
    /// <summary>
    /// Interaction logic for WindowThemPhim.xaml
    /// </summary>
    public partial class WindowThemPhim : Window
    {
        public bool isEdit = false;
        public int MaPhim;

        public WindowThemPhim()
        {
            InitializeComponent();
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DBRapPhimEntities2 db = new DBRapPhimEntities2())
                {
                    if (!isEdit)
                    {
                        phim p = new phim();

                        GanDuLieu(p);

                        db.phims.Add(p);
                        db.SaveChanges();
                    }
                    else
                    {
                        var p = db.phims.Find(MaPhim);

                        if (p != null)
                        {
                            GanDuLieu(p);
                            db.SaveChanges();
                        }
                    }
                }

                MessageBox.Show("Lưu thành công!");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void GanDuLieu(phim p)
        {
            p.ten_phim = txtTenPhim.Text;
            p.ma_the_loai = int.Parse(txtMaTheLoai.Text);
            p.thoi_luong = int.Parse(txtThoiLuong.Text);

            if (dpNgayKhoiChieu.SelectedDate != null)
                p.ngay_khoi_chieu = dpNgayKhoiChieu.SelectedDate.Value;

            if (dpNgayKetThuc.SelectedDate != null)
                p.ngay_ket_thuc = dpNgayKetThuc.SelectedDate.Value;

            p.mo_ta = txtMoTa.Text;

            if (cbTrangThai.SelectedItem != null)
            {
                ComboBoxItem item = (ComboBoxItem)cbTrangThai.SelectedItem;
                p.trang_thai = item.Content.ToString();
            }

            if (!string.IsNullOrEmpty(txtNguoiNhap.Text))
                p.nguoi_nhap = int.Parse(txtNguoiNhap.Text);
        }
    }
}
