using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cinema
{
    public partial class qlsp : Page
    {
        // ĐÃ SỬA: Đổi thành DBRapPhimEntities2 theo đúng tên máy Mạnh
        DBRapPhimEntities2 db = new DBRapPhimEntities2();

        public qlsp()
        {
            InitializeComponent();
            LoadDuLieu();
        }

        private void LoadDuLieu()
        {
            try
            {
                // ĐÃ SỬA: Đổi thành DBRapPhimEntities2
                db = new DBRapPhimEntities2();
                dgProducts.ItemsSource = db.sanphams.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị: " + ex.Message);
            }
        }

        // --- HÀM PHIÊN DỊCH: Từ Giao diện -> Database ---
        private string GetLoaiSQL(string uiText)
        {
            if (uiText == "Nước ngọt") return "NuocUong";
            if (uiText == "Combo") return "Combo";
            return "DoAn";
        }

        private string GetTrangThaiSQL(string uiText)
        {
            if (uiText == "Ngừng kinh doanh") return "NgungBan";
            return "DangBan";
        }

        // --- HÀM PHIÊN DỊCH: Từ Database -> Giao diện ---
        private string GetLoaiUI(string sqlText)
        {
            if (sqlText == "NuocUong") return "Nước ngọt";
            if (sqlText == "Combo") return "Combo";
            return "Bắp rang";
        }

        private string GetTrangThaiUI(string sqlText)
        {
            if (sqlText == "NgungBan") return "Ngừng kinh doanh";
            return "Đang kinh doanh";
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên và Giá!", "Thông báo");
                return;
            }

            try
            {
                string uiLoai = (cmbCategory.SelectedItem as ComboBoxItem)?.Content.ToString();
                string uiTrangThai = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

                var spMoi = new sanpham()
                {
                    ten_san_pham = txtName.Text,
                    gia_ban = decimal.Parse(txtPrice.Text),
                    so_luong_ton = string.IsNullOrWhiteSpace(txtQuantity.Text) ? 0 : int.Parse(txtQuantity.Text),
                    loai = GetLoaiSQL(uiLoai),
                    trang_thai = GetTrangThaiSQL(uiTrangThai)
                };

                db.sanphams.Add(spMoi);
                db.SaveChanges();
                LoadDuLieu();
                btnClear_Click(null, null);
                MessageBox.Show("Thêm thành công!", "Thông báo");
            }
            catch (Exception ex)
            {
                Exception loiGoc = ex;
                while (loiGoc.InnerException != null) { loiGoc = loiGoc.InnerException; }
                MessageBox.Show("CHI TIẾT LỖI TỪ SQL:\n" + loiGoc.Message, "Lỗi Database");
            }
        }

        private void dgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProducts.SelectedItem is sanpham selected)
            {
                txtId.Text = selected.ma_san_pham.ToString();
                txtName.Text = selected.ten_san_pham;
                txtPrice.Text = selected.gia_ban.ToString();
                txtQuantity.Text = selected.so_luong_ton.ToString();

                string uiLoai = GetLoaiUI(selected.loai);
                foreach (ComboBoxItem item in cmbCategory.Items)
                    if (item.Content.ToString() == uiLoai) { cmbCategory.SelectedItem = item; break; }

                string uiTrangThai = GetTrangThaiUI(selected.trang_thai);
                foreach (ComboBoxItem item in cmbStatus.Items)
                    if (item.Content.ToString() == uiTrangThai) { cmbStatus.SelectedItem = item; break; }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text)) return;
            try
            {
                int maSP = int.Parse(txtId.Text);
                var spSua = db.sanphams.FirstOrDefault(s => s.ma_san_pham == maSP);
                if (spSua != null)
                {
                    string uiLoai = (cmbCategory.SelectedItem as ComboBoxItem)?.Content.ToString();
                    string uiTrangThai = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

                    spSua.ten_san_pham = txtName.Text;
                    spSua.gia_ban = decimal.Parse(txtPrice.Text);
                    spSua.so_luong_ton = string.IsNullOrWhiteSpace(txtQuantity.Text) ? 0 : int.Parse(txtQuantity.Text);
                    spSua.loai = GetLoaiSQL(uiLoai);
                    spSua.trang_thai = GetTrangThaiSQL(uiTrangThai);

                    db.SaveChanges();
                    LoadDuLieu();
                    MessageBox.Show("Cập nhật thành công!");
                }
            }
            catch (Exception ex)
            {
                Exception loiGoc = ex;
                while (loiGoc.InnerException != null) { loiGoc = loiGoc.InnerException; }
                MessageBox.Show("CHI TIẾT LỖI TỪ SQL:\n" + loiGoc.Message, "Lỗi Database");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text)) return;
            if (MessageBox.Show("Bạn có muốn xóa không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    int maSP = int.Parse(txtId.Text);
                    var spXoa = db.sanphams.FirstOrDefault(s => s.ma_san_pham == maSP);
                    if (spXoa != null)
                    {
                        db.sanphams.Remove(spXoa);
                        db.SaveChanges();
                        LoadDuLieu();
                        btnClear_Click(null, null);
                    }
                }
                catch (Exception ex)
                {
                    Exception loiGoc = ex;
                    while (loiGoc.InnerException != null) { loiGoc = loiGoc.InnerException; }
                    MessageBox.Show("CHI TIẾT LỖI TỪ SQL:\n" + loiGoc.Message, "Lỗi Database");
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtId.Clear();
            txtName.Clear();
            txtPrice.Clear();
            if (txtQuantity != null) txtQuantity.Clear();
            cmbCategory.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;
        }
    }
}