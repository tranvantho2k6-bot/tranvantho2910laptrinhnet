using System.Data;
using System.Text;
using HotelMS.BUS;
using HotelMS.DTO;
namespace HotelMS.GUI;

public class MainForm : Form
{
    // Tab phòng
    private DataGridView dgvPhong;
    private ComboBox cboLoai, cboTT;
    private TextBox txtTim;
    private DataTable dtPhong;
    private DataView viewPhong;
    private bool loadingFilter;
    // Tab check-in
    private TextBox txtCCCD, txtTen, txtSDT;
    private Label lblKH;
    private ComboBox cboPhongTrong;
    private int maKHHienTai = 0;
    // Tab dịch vụ
    private ComboBox cboPhieuDV, cboDV;
    private NumericUpDown nudSL;
    private DataGridView dgvDV;
    private Label lblTongDV;
    // Tab check-out
    private ComboBox cboPhieuCO;
    private TextBox txtBill;

    public MainForm()
    {
        Text = "Hotel Management System";
        ClientSize = new Size(1100, 640);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        var tabs = new TabControl { Dock = DockStyle.Fill };
        var t1 = new TabPage("Danh sách phòng");
        var t2 = new TabPage("Check-in");
        var t3 = new TabPage("Gọi dịch vụ");
        var t4 = new TabPage("Check-out & Hóa đơn");
        tabs.TabPages.AddRange(new[] { t1, t2, t3, t4 });
        Controls.Add(tabs);

        BuildTabPhong(t1);
        BuildTabCheckIn(t2);
        BuildTabDichVu(t3);
        BuildTabCheckOut(t4);

        Load += (s, e) => { try { RefreshAll(); } catch (Exception ex) { MessageBox.Show("Không tải được dữ liệu từ CSDL.\n\nHãy kiểm tra: (1) đã bật MySQL trong XAMPP Control Panel chưa, (2) đã import Database/ScriptDB.sql bằng phpMyAdmin chưa, (3) chuỗi kết nối trong App.config (Server, Port, User ID, Password) có đúng không.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi kết nối CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error); } };
    }

    // ================= TAB PHÒNG =================
    private void BuildTabPhong(TabPage t)
    {
        Ui.AddLabel(t, "Loại phòng", 10, 12);
        cboLoai = Ui.AddCombo(t, 85, 10, 120);
        Ui.AddLabel(t, "Trạng thái", 225, 12);
        cboTT = Ui.AddCombo(t, 295, 10, 130);
        cboTT.Items.AddRange(new object[] { "Tất cả", "Trống", "Đang ở", "Đang dọn dẹp" });
        cboTT.SelectedIndex = 0;
        Ui.AddLabel(t, "Số phòng", 445, 12);
        txtTim = Ui.AddText(t, 515, 10, 110);
        Ui.AddButton(t, "Bỏ lọc", 640, 8, 80, (s, e) => { loadingFilter = true; cboLoai.SelectedIndex = 0; cboTT.SelectedIndex = 0; txtTim.Clear(); loadingFilter = false; ApplyFilter(); });
        Ui.AddButton(t, "Đánh dấu đã dọn xong", 740, 8, 170, (s, e) => Run(() =>
        {
            string so = dgvPhong.CurrentRow?.Cells["SoPhong"].Value?.ToString();
            PhongBUS.DonXong(so);
            RefreshAll();
        }));
        dgvPhong = Ui.AddGrid(t, 10, 50, 1060, 530);

        cboLoai.SelectedIndexChanged += (s, e) => ApplyFilter();
        cboTT.SelectedIndexChanged += (s, e) => ApplyFilter();
        txtTim.TextChanged += (s, e) => ApplyFilter();
    }

    private static string Esc(string s) => s.Replace("'", "''");

    // Lọc ngắt kết nối bằng DataView.RowFilter
    private void ApplyFilter()
    {
        if (loadingFilter || viewPhong == null) return;
        var f = new List<string>();
        if (cboLoai.SelectedIndex > 0) f.Add($"LoaiPhong = '{Esc(cboLoai.Text)}'");
        if (cboTT.SelectedIndex > 0) f.Add($"TrangThai = '{Esc(cboTT.Text)}'");
        if (txtTim.Text.Trim() != "") f.Add($"SoPhong LIKE '%{Esc(txtTim.Text.Trim())}%'");
        viewPhong.RowFilter = string.Join(" AND ", f);
    }

    private void LoadPhong()
    {
        loadingFilter = true;
        dtPhong = PhongBUS.GetAll();
        viewPhong = new DataView(dtPhong);
        dgvPhong.DataSource = viewPhong;
        dgvPhong.Columns["GiaTheoGio"].DefaultCellStyle.Format = "N0";

        string old = cboLoai.Text;
        cboLoai.Items.Clear();
        cboLoai.Items.Add("Tất cả");
        foreach (var s in dtPhong.AsEnumerable().Select(r => r.Field<string>("LoaiPhong")).Distinct())
            cboLoai.Items.Add(s);
        int idx = cboLoai.Items.IndexOf(old);
        cboLoai.SelectedIndex = idx < 0 ? 0 : idx;
        loadingFilter = false;
        ApplyFilter();
    }

    // ================= TAB CHECK-IN =================
    private void BuildTabCheckIn(TabPage t)
    {
        Ui.AddLabel(t, "Số CCCD", 20, 25);
        txtCCCD = Ui.AddText(t, 130, 22, 200);
        Ui.AddButton(t, "Tra cứu", 345, 20, 100, (s, e) => Run(TraCuuKhach));
        lblKH = Ui.AddLabel(t, "Nhập CCCD rồi bấm Tra cứu.", 130, 60);
        Ui.AddLabel(t, "Họ tên", 20, 100);
        txtTen = Ui.AddText(t, 130, 97, 300);
        Ui.AddLabel(t, "Số điện thoại", 20, 140);
        txtSDT = Ui.AddText(t, 130, 137, 200);
        Ui.AddLabel(t, "Phòng trống", 20, 190);
        cboPhongTrong = Ui.AddCombo(t, 130, 187, 300);
        Ui.AddButton(t, "Nhận phòng (Check-in)", 130, 235, 200, (s, e) => Run(CheckIn));
        txtCCCD.TextChanged += (s, e) => { maKHHienTai = 0; txtTen.ReadOnly = false; txtSDT.ReadOnly = false; };
    }

    private void TraCuuKhach()
    {
        var kh = KhachHangBUS.TimTheoCCCD(txtCCCD.Text);
        if (kh == null)
        {
            maKHHienTai = 0;
            txtTen.Clear(); txtSDT.Clear();
            txtTen.ReadOnly = false; txtSDT.ReadOnly = false;
            lblKH.Text = "Khách mới - vui lòng nhập họ tên, số điện thoại.";
        }
        else
        {
            maKHHienTai = kh.MaKH;
            txtTen.Text = kh.HoTen; txtSDT.Text = kh.SoDienThoai;
            txtTen.ReadOnly = true; txtSDT.ReadOnly = true;
            lblKH.Text = "Khách đã có trong hệ thống (Mã KH: " + kh.MaKH + ").";
        }
    }

    private void CheckIn()
    {
        var kh = new KhachHangDTO
        {
            MaKH = maKHHienTai,
            CCCD = txtCCCD.Text.Trim(),
            HoTen = txtTen.Text.Trim(),
            SoDienThoai = txtSDT.Text.Trim()
        };
        string phong = cboPhongTrong.SelectedValue?.ToString();
        int maPhieu = LuuTruBUS.CheckIn(kh, phong);
        MessageBox.Show("Check-in thành công. Mã phiếu: " + maPhieu, "Thành công");
        txtCCCD.Clear(); txtTen.Clear(); txtSDT.Clear(); lblKH.Text = "";
        RefreshAll();
    }

    // ================= TAB DỊCH VỤ =================
    private void BuildTabDichVu(TabPage t)
    {
        Ui.AddLabel(t, "Phòng đang ở", 20, 25);
        cboPhieuDV = Ui.AddCombo(t, 130, 22, 300);
        Ui.AddLabel(t, "Dịch vụ", 20, 65);
        cboDV = Ui.AddCombo(t, 130, 62, 300);
        Ui.AddLabel(t, "Số lượng", 20, 105);
        nudSL = Ui.AddNum(t, 130, 102, 80, 1, 1, 100, 0);
        Ui.AddButton(t, "Thêm dịch vụ", 230, 100, 130, (s, e) => Run(GoiDichVu));
        dgvDV = Ui.AddGrid(t, 20, 150, 1040, 380);
        lblTongDV = Ui.AddLabel(t, "Tổng tiền dịch vụ: 0 đ", 20, 545);
        lblTongDV.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        cboPhieuDV.SelectedIndexChanged += (s, e) => LoadDV();
    }

    private void GoiDichVu()
    {
        var dv = cboDV.SelectedItem as ServiceDTO;
        var d = new SuDungDichVuDTO
        {
            MaPhieu = cboPhieuDV.SelectedValue is int p ? p : 0,
            MaDV = dv?.MaDV ?? 0,
            SoLuong = (int)nudSL.Value
        };
        DichVuBUS.GoiDichVu(d);
        LoadDV();
    }

    private void LoadDV()
    {
        if (cboPhieuDV.SelectedValue is int p)
        {
            var dt = DichVuBUS.GetByPhieu(p);
            dgvDV.DataSource = dt;
            dgvDV.Columns["Đơn giá"].DefaultCellStyle.Format = "N0";
            dgvDV.Columns["Thành tiền"].DefaultCellStyle.Format = "N0";
            decimal tong = 0;
            foreach (DataRow r in dt.Rows) tong += Convert.ToDecimal(r["Thành tiền"]);
            lblTongDV.Text = "Tổng tiền dịch vụ: " + tong.ToString("N0") + " đ";
        }
        else
        {
            dgvDV.DataSource = null;
            lblTongDV.Text = "Tổng tiền dịch vụ: 0 đ";
        }
    }

    // ================= TAB CHECK-OUT =================
    private void BuildTabCheckOut(TabPage t)
    {
        Ui.AddLabel(t, "Phòng đang ở", 20, 25);
        cboPhieuCO = Ui.AddCombo(t, 130, 22, 300);
        Ui.AddButton(t, "Xem hóa đơn", 450, 20, 130, (s, e) => Run(XemHoaDon));
        Ui.AddButton(t, "Xác nhận thanh toán & Check-out", 595, 20, 260, (s, e) => Run(CheckOut));
        txtBill = new TextBox
        {
            Left = 20, Top = 70, Width = 700, Height = 480, Multiline = true, ReadOnly = true,
            Font = new Font("Consolas", 11), ScrollBars = ScrollBars.Vertical
        };
        t.Controls.Add(txtBill);
    }

    private static string FormatBill(BillDTO b, string title)
    {
        var sb = new StringBuilder();
        sb.AppendLine(title);
        sb.AppendLine(new string('=', 52));
        sb.AppendLine($"Mã phiếu      : {b.MaPhieu}");
        sb.AppendLine($"Phòng         : {b.SoPhong}");
        sb.AppendLine($"Khách hàng    : {b.HoTen}");
        sb.AppendLine($"Nhận phòng    : {b.NgayNhan:dd/MM/yyyy HH:mm}");
        sb.AppendLine($"Trả phòng     : {b.NgayTra:dd/MM/yyyy HH:mm}");
        sb.AppendLine(new string('-', 52));
        sb.AppendLine($"Số giờ tính   : {b.SoGio} giờ x {b.GiaTheoGio:N0} đ");
        sb.AppendLine($"Tiền phòng    : {b.TienPhong:N0} đ");
        sb.AppendLine($"Tiền dịch vụ  : {b.TienDichVu:N0} đ");
        sb.AppendLine(new string('=', 52));
        sb.AppendLine($"TỔNG THANH TOÁN: {b.TongTien:N0} đ");
        return sb.ToString();
    }

    private void XemHoaDon()
    {
        int p = cboPhieuCO.SelectedValue is int x ? x : throw new ArgumentException("Vui lòng chọn phòng đang ở.");
        txtBill.Text = FormatBill(LuuTruBUS.TinhHoaDon(p, DateTime.Now), "HÓA ĐƠN TẠM TÍNH");
    }

    private void CheckOut()
    {
        int p = cboPhieuCO.SelectedValue is int x ? x : throw new ArgumentException("Vui lòng chọn phòng đang ở.");
        var tam = LuuTruBUS.TinhHoaDon(p, DateTime.Now);
        if (MessageBox.Show($"Tổng thanh toán: {tam.TongTien:N0} đ\nXác nhận check-out phòng {tam.SoPhong}?",
                "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        var bill = LuuTruBUS.CheckOut(p);
        txtBill.Text = FormatBill(bill, "HÓA ĐƠN THANH TOÁN");
        RefreshAll();
    }

    // ================= CHUNG =================
    private void RefreshAll()
    {
        LoadPhong();

        cboPhongTrong.DisplayMember = "MoTa";
        cboPhongTrong.ValueMember = "SoPhong";
        cboPhongTrong.DataSource = PhongBUS.GetPhongTrong();

        var dt = LuuTruBUS.GetPhieuDangO();
        cboPhieuDV.DisplayMember = "MoTa"; cboPhieuDV.ValueMember = "MaPhieu";
        cboPhieuDV.DataSource = dt;
        cboPhieuCO.DisplayMember = "MoTa"; cboPhieuCO.ValueMember = "MaPhieu";
        cboPhieuCO.DataSource = dt.Copy();

        if (cboDV.Items.Count == 0)
            foreach (var d in DichVuBUS.GetAll()) cboDV.Items.Add(d);
        if (cboDV.SelectedIndex < 0 && cboDV.Items.Count > 0) cboDV.SelectedIndex = 0;
        LoadDV();
    }

    private void Run(Action a)
    {
        try { a(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}
