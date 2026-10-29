using System.Data;
using InventoryMS.BUS;
using InventoryMS.DTO;
namespace InventoryMS.GUI;

public class MainForm : Form
{
    // Vật tư
    private DataGridView dgvVT;
    private TextBox txtMa, txtTenVT, txtDVT, txtTimVT;
    private NumericUpDown nudTon;
    // Nhà cung cấp
    private DataGridView dgvNCC;
    private TextBox txtTenNCC, txtDT, txtDC, txtTimNCC;
    // Phiếu kho
    private ComboBox cboLoai, cboNCC, cboVT;
    private NumericUpDown nudSLPhieu;
    private TextBox txtDonGia;
    private DataGridView dgvCart;
    private DataTable cart;
    private Label lblTongPhieu;
    // Báo cáo
    private NumericUpDown nudNguong;
    private DataGridView dgvTonThap;
    private Label lblTongGiaTri;

    public MainForm()
    {
        Text = "Inventory Management - Quản lý xuất nhập kho";
        ClientSize = new Size(1150, 660);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        var tabs = new TabControl { Dock = DockStyle.Fill };
        var t1 = new TabPage("Vật tư");
        var t2 = new TabPage("Nhà cung cấp");
        var t3 = new TabPage("Lập phiếu nhập / xuất");
        var t4 = new TabPage("Cảnh báo tồn kho & Báo cáo");
        tabs.TabPages.AddRange(new[] { t1, t2, t3, t4 });
        Controls.Add(tabs);

        BuildVatTu(t1);
        BuildNCC(t2);
        BuildPhieu(t3);
        BuildBaoCao(t4);
        Load += (s, e) => { try { LoadAll(); } catch (Exception ex) { MessageBox.Show("Không tải được dữ liệu từ CSDL.\n\nHãy kiểm tra: (1) đã bật MySQL trong XAMPP Control Panel chưa, (2) đã import Database/ScriptDB.sql bằng phpMyAdmin chưa, (3) chuỗi kết nối trong App.config (Server, Port, User ID, Password) có đúng không.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi kết nối CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error); } };
    }

    private void Run(Action a)
    {
        try { a(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private bool Confirm() =>
        MessageBox.Show("Bạn chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes;

    private void LoadAll()
    {
        LoadVatTu(null);
        LoadNCC(null);
        LoadComboPhieu();
    }

    // ============ VẬT TƯ ============
    private void BuildVatTu(TabPage t)
    {
        Ui.AddLabel(t, "Mã VT", 10, 15);
        txtMa = Ui.AddText(t, 60, 12, 110);
        Ui.AddLabel(t, "Tên vật tư", 185, 15);
        txtTenVT = Ui.AddText(t, 255, 12, 230);
        Ui.AddLabel(t, "ĐVT", 500, 15);
        txtDVT = Ui.AddText(t, 535, 12, 80);
        Ui.AddLabel(t, "Tồn đầu", 630, 15);
        nudTon = Ui.AddNum(t, 690, 12, 90, 0, 0, 1000000000, 2);
        Ui.AddButton(t, "Thêm", 800, 10, 80, (s, e) => Run(() => { VatTuBUS.Them(ReadVT()); LoadVatTu(null); }));
        Ui.AddButton(t, "Sửa", 890, 10, 80, (s, e) => Run(() => { VatTuBUS.Sua(ReadVT()); LoadVatTu(null); }));
        Ui.AddButton(t, "Xóa", 980, 10, 80, (s, e) => Run(() => { if (Confirm()) { VatTuBUS.Xoa(txtMa.Text); LoadVatTu(null); } }));
        Ui.AddLabel(t, "Tìm (mã/tên)", 10, 58);
        txtTimVT = Ui.AddText(t, 100, 55, 250);
        Ui.AddButton(t, "Tìm", 360, 53, 80, (s, e) => Run(() => LoadVatTu(txtTimVT.Text)));
        Ui.AddButton(t, "Tất cả", 450, 53, 80, (s, e) => Run(() => { txtTimVT.Clear(); LoadVatTu(null); }));
        dgvVT = Ui.AddGrid(t, 10, 95, 1110, 480);
        dgvVT.SelectionChanged += (s, e) =>
        {
            if (dgvVT.CurrentRow?.DataBoundItem is DataRowView v)
            {
                txtMa.Text = v["MaVatTu"].ToString();
                txtTenVT.Text = v["TenVatTu"].ToString();
                txtDVT.Text = v["DVT"].ToString();
                nudTon.Value = Convert.ToDecimal(v["SoLuongTon"]);
            }
        };
    }

    private VatTuDTO ReadVT() => new VatTuDTO
    {
        MaVatTu = txtMa.Text, TenVatTu = txtTenVT.Text, DVT = txtDVT.Text.Trim(), SoLuongTon = nudTon.Value
    };

    private void LoadVatTu(string kw)
    {
        dgvVT.DataSource = kw == null ? VatTuBUS.GetAll() : VatTuBUS.Search(kw);
    }

    // ============ NHÀ CUNG CẤP ============
    private void BuildNCC(TabPage t)
    {
        Ui.AddLabel(t, "Tên NCC", 10, 15);
        txtTenNCC = Ui.AddText(t, 75, 12, 260);
        Ui.AddLabel(t, "Điện thoại", 350, 15);
        txtDT = Ui.AddText(t, 425, 12, 120);
        Ui.AddLabel(t, "Địa chỉ", 560, 15);
        txtDC = Ui.AddText(t, 615, 12, 190);
        Ui.AddButton(t, "Thêm", 820, 10, 80, (s, e) => Run(() => { NhaCungCapBUS.Them(ReadNCC(0)); LoadNCC(null); }));
        Ui.AddButton(t, "Sửa", 910, 10, 80, (s, e) => Run(() => { NhaCungCapBUS.Sua(ReadNCC(SelectedNCC())); LoadNCC(null); }));
        Ui.AddButton(t, "Xóa", 1000, 10, 80, (s, e) => Run(() => { if (Confirm()) { NhaCungCapBUS.Xoa(SelectedNCC()); LoadNCC(null); LoadComboPhieu(); } }));
        Ui.AddLabel(t, "Tìm (tên/SĐT)", 10, 58);
        txtTimNCC = Ui.AddText(t, 100, 55, 250);
        Ui.AddButton(t, "Tìm", 360, 53, 80, (s, e) => Run(() => LoadNCC(txtTimNCC.Text)));
        Ui.AddButton(t, "Tất cả", 450, 53, 80, (s, e) => Run(() => { txtTimNCC.Clear(); LoadNCC(null); }));
        dgvNCC = Ui.AddGrid(t, 10, 95, 1110, 480);
        dgvNCC.SelectionChanged += (s, e) =>
        {
            if (dgvNCC.CurrentRow?.DataBoundItem is DataRowView v)
            {
                txtTenNCC.Text = v["TenNCC"].ToString();
                txtDT.Text = v["DienThoai"].ToString();
                txtDC.Text = v["DiaChi"].ToString();
            }
        };
    }

    private int SelectedNCC() =>
        dgvNCC.CurrentRow?.DataBoundItem is DataRowView v ? Convert.ToInt32(v["MaNCC"]) : 0;

    private NhaCungCapDTO ReadNCC(int ma) => new NhaCungCapDTO
    {
        MaNCC = ma, TenNCC = txtTenNCC.Text, DienThoai = txtDT.Text, DiaChi = txtDC.Text.Trim()
    };

    private void LoadNCC(string kw)
    {
        dgvNCC.DataSource = kw == null ? NhaCungCapBUS.GetAll() : NhaCungCapBUS.Search(kw);
        dgvNCC.Columns["MaNCC"].Visible = false;
    }

    // ============ LẬP PHIẾU KHO ============
    private void BuildPhieu(TabPage t)
    {
        Ui.AddLabel(t, "Loại phiếu", 10, 15);
        cboLoai = Ui.AddCombo(t, 85, 12, 90);
        cboLoai.Items.AddRange(new object[] { "NHAP", "XUAT" });
        cboLoai.SelectedIndex = 0;
        Ui.AddLabel(t, "Nhà cung cấp", 195, 15);
        cboNCC = Ui.AddCombo(t, 285, 12, 280);
        Ui.AddLabel(t, "Vật tư", 10, 60);
        cboVT = Ui.AddCombo(t, 85, 57, 330);
        Ui.AddLabel(t, "Số lượng", 430, 60);
        nudSLPhieu = Ui.AddNum(t, 495, 57, 90, 1, 0.01m, 1000000, 2);
        Ui.AddLabel(t, "Đơn giá", 600, 60);
        txtDonGia = Ui.AddText(t, 655, 57, 100);
        txtDonGia.Text = "0";
        Ui.AddButton(t, "Thêm vào phiếu", 770, 55, 130, (s, e) => Run(ThemVaoPhieu));
        Ui.AddButton(t, "Xóa dòng chọn", 910, 55, 130, (s, e) => Run(() =>
        {
            if (dgvCart.CurrentRow?.DataBoundItem is DataRowView v) { v.Row.Delete(); cart.AcceptChanges(); CapNhatTong(); }
        }));

        cart = new DataTable();
        cart.Columns.Add("MaVatTu", typeof(string));
        cart.Columns.Add("TenVatTu", typeof(string));
        cart.Columns.Add("SoLuong", typeof(decimal));
        cart.Columns.Add("DonGia", typeof(decimal));
        cart.Columns.Add("ThanhTien", typeof(decimal), "SoLuong * DonGia");
        dgvCart = Ui.AddGrid(t, 10, 100, 1110, 410);
        dgvCart.DataSource = cart;

        lblTongPhieu = Ui.AddLabel(t, "Tổng tiền phiếu: 0 đ", 10, 530);
        lblTongPhieu.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        Ui.AddButton(t, "Lưu phiếu kho", 940, 522, 180, (s, e) => Run(LuuPhieu));

        cboLoai.SelectedIndexChanged += (s, e) => cboNCC.Enabled = cboLoai.Text == "NHAP";
    }

    private void LoadComboPhieu()
    {
        cboNCC.DisplayMember = "TenNCC";
        cboNCC.ValueMember = "MaNCC";
        cboNCC.DataSource = NhaCungCapBUS.GetAll();
        cboVT.DisplayMember = "MoTa";
        cboVT.ValueMember = "MaVatTu";
        cboVT.DataSource = VatTuBUS.GetForCombo();
    }

    private void ThemVaoPhieu()
    {
        if (cboVT.SelectedItem is not DataRowView vt) throw new ArgumentException("Vui lòng chọn vật tư.");
        if (!decimal.TryParse(txtDonGia.Text.Trim(), out var gia) || gia < 0)
            throw new ArgumentException("Đơn giá phải là số không âm.");
        string ma = vt["MaVatTu"].ToString();
        var exist = cart.Select("MaVatTu = '" + ma.Replace("'", "''") + "'");
        if (exist.Length > 0)
        {
            exist[0]["SoLuong"] = (decimal)exist[0]["SoLuong"] + nudSLPhieu.Value;
            exist[0]["DonGia"] = gia;
        }
        else
        {
            cart.Rows.Add(ma, vt["TenVatTu"].ToString(), nudSLPhieu.Value, gia);
        }
        CapNhatTong();
    }

    private void CapNhatTong()
    {
        decimal tong = 0;
        foreach (DataRow r in cart.Rows) tong += (decimal)r["ThanhTien"];
        lblTongPhieu.Text = "Tổng tiền phiếu: " + tong.ToString("N0") + " đ";
    }

    private void LuuPhieu()
    {
        var p = new PhieuKhoDTO
        {
            LoaiPhieu = cboLoai.Text,
            MaNCC = cboLoai.Text == "NHAP" && cboNCC.SelectedValue is int n ? n : (int?)null
        };
        foreach (DataRow r in cart.Rows)
            p.ChiTiet.Add(new ChiTietPhieuKhoDTO
            {
                MaVatTu = r["MaVatTu"].ToString(),
                SoLuong = (decimal)r["SoLuong"],
                DonGia = (decimal)r["DonGia"]
            });
        try
        {
            int ma = PhieuKhoBUS.LuuPhieu(p);
            MessageBox.Show("Đã lưu phiếu " + p.LoaiPhieu + " số " + ma, "Thành công");
            cart.Clear();
            CapNhatTong();
            LoadVatTu(null);
            LoadComboPhieu();
        }
        catch (InvalidOperationException ex)
        {
            // Giao dịch đã Rollback (ví dụ: Không đủ hàng trong kho)
            MessageBox.Show(ex.Message + "\nGiao dịch đã được Rollback, dữ liệu kho không thay đổi.", "Lỗi giao dịch",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            LoadVatTu(null);
            LoadComboPhieu();
        }
    }

    // ============ BÁO CÁO ============
    private void BuildBaoCao(TabPage t)
    {
        Ui.AddLabel(t, "Ngưỡng an toàn (tồn <)", 10, 15);
        nudNguong = Ui.AddNum(t, 170, 12, 90, 10, 0, 1000000, 2);
        Ui.AddButton(t, "Lọc vật tư dưới ngưỡng", 275, 10, 190, (s, e) => Run(() => dgvTonThap.DataSource = VatTuBUS.GetTonThap(nudNguong.Value)));
        Ui.AddButton(t, "Tính tổng giá trị tồn kho", 490, 10, 210, (s, e) => Run(() =>
            lblTongGiaTri.Text = "Tổng giá trị hàng tồn kho: " + VatTuBUS.TongGiaTriTon().ToString("N0") + " đ"));
        lblTongGiaTri = Ui.AddLabel(t, "Tổng giá trị hàng tồn kho: (chưa tính)", 10, 60);
        lblTongGiaTri.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        dgvTonThap = Ui.AddGrid(t, 10, 100, 1110, 470);
    }
}
