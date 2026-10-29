using System.Data;
using CafePOS.BUS;
namespace CafePOS.GUI;

public class MainForm : Form
{
    private readonly FlowLayoutPanel pnlBan = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(8) };
    private DataGridView dgv;
    private ComboBox cboDM, cboMon, cboBanMoi;
    private NumericUpDown nudSL;
    private Label lblBan, lblTong;
    private int maBanChon = 0;
    private string tenBanChon = "";
    private DataTable dtMon;

    public MainForm()
    {
        Text = "Cafe POS - Quản lý theo sơ đồ bàn";
        ClientSize = new Size(1180, 680);
        StartPosition = FormStartPosition.CenterScreen;

        var grp = new GroupBox { Text = "Sơ đồ bàn  (Xanh = Trống, Đỏ = Có khách)", Dock = DockStyle.Fill };
        grp.Controls.Add(pnlBan);
        var pnlRight = new Panel { Dock = DockStyle.Right, Width = 560 };
        Controls.Add(grp);
        Controls.Add(pnlRight);

        lblBan = Ui.AddLabel(pnlRight, "Chưa chọn bàn", 10, 10);
        lblBan.Font = new Font("Segoe UI", 13, FontStyle.Bold);
        Ui.AddLabel(pnlRight, "Danh mục", 10, 55);
        cboDM = Ui.AddCombo(pnlRight, 80, 52, 180);
        Ui.AddLabel(pnlRight, "Món", 280, 55);
        cboMon = Ui.AddCombo(pnlRight, 320, 52, 220);
        Ui.AddLabel(pnlRight, "Số lượng", 10, 95);
        nudSL = Ui.AddNum(pnlRight, 80, 92, 80, 1, 1, 100, 0);
        Ui.AddButton(pnlRight, "Thêm món", 180, 90, 120, (s, e) => ThemMon());
        dgv = Ui.AddGrid(pnlRight, 10, 140, 535, 310);
        lblTong = Ui.AddLabel(pnlRight, "Tổng tiền: 0 đ", 10, 465);
        lblTong.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        Ui.AddButton(pnlRight, "Thanh toán", 385, 460, 160, (s, e) => ThanhToan());
        Ui.AddLabel(pnlRight, "Đổi sang bàn", 10, 520);
        cboBanMoi = Ui.AddCombo(pnlRight, 100, 517, 160);
        Ui.AddButton(pnlRight, "Đổi bàn", 270, 515, 100, (s, e) => DoiBan());
        Ui.AddButton(pnlRight, "Quản lý thực đơn", 385, 515, 160, (s, e) => MoQuanLyMenu());

        cboDM.SelectedIndexChanged += (s, e) => FilterMon();
        Load += (s, e) => { try { LoadMenu(); LoadBan(); } catch (Exception ex) { MessageBox.Show("Không tải được dữ liệu từ CSDL.\n\nHãy kiểm tra: (1) đã bật MySQL trong XAMPP Control Panel chưa, (2) đã import Database/ScriptDB.sql bằng phpMyAdmin chưa, (3) chuỗi kết nối trong App.config (Server, Port, User ID, Password) có đúng không.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi kết nối CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error); } };
    }

    private void LoadMenu()
    {
        dtMon = MenuBUS.GetMonAn();
        cboDM.DisplayMember = "TenDanhMuc";
        cboDM.ValueMember = "MaDanhMuc";
        cboDM.DataSource = MenuBUS.GetDanhMuc();
        FilterMon();
    }

    private void FilterMon()
    {
        if (dtMon == null) return;
        if (cboDM.SelectedValue is int id)
        {
            var view = new DataView(dtMon) { RowFilter = $"MaDanhMuc = {id}" };
            cboMon.DisplayMember = "TenMon";
            cboMon.ValueMember = "MaMon";
            cboMon.DataSource = view;
        }
    }

    private void LoadBan()
    {
        var dt = BanAnBUS.GetAll();
        pnlBan.Controls.Clear();
        foreach (DataRow r in dt.Rows)
        {
            int ma = (int)r["MaBan"];
            string ten = r["TenBan"].ToString();
            string tt = r["TrangThai"].ToString();
            var b = new Button
            {
                Width = 120, Height = 90, Margin = new Padding(8),
                Text = ten + Environment.NewLine + tt,
                Tag = ma, FlatStyle = FlatStyle.Flat, ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = tt == "Trống" ? Color.SeaGreen : Color.Firebrick
            };
            if (ma == maBanChon) { b.FlatAppearance.BorderColor = Color.Gold; b.FlatAppearance.BorderSize = 4; }
            b.Click += (s, e) => { maBanChon = ma; tenBanChon = ten; LoadBan(); LoadChiTiet(); };
            pnlBan.Controls.Add(b);
        }
        var trong = new DataView(dt) { RowFilter = "TrangThai = 'Trống'" };
        cboBanMoi.DisplayMember = "TenBan";
        cboBanMoi.ValueMember = "MaBan";
        cboBanMoi.DataSource = trong;
    }

    private void LoadChiTiet()
    {
        lblBan.Text = maBanChon == 0 ? "Chưa chọn bàn" : "Hóa đơn: " + tenBanChon;
        decimal tong = 0;
        var maHD = maBanChon == 0 ? null : HoaDonBUS.GetHoaDonMo(maBanChon);
        if (maHD == null)
        {
            dgv.DataSource = null;
        }
        else
        {
            var dt = HoaDonBUS.GetChiTiet(maHD.Value);
            dgv.DataSource = dt;
            dgv.Columns["MaMon"].Visible = false;
            dgv.Columns["Đơn giá"].DefaultCellStyle.Format = "N0";
            dgv.Columns["Thành tiền"].DefaultCellStyle.Format = "N0";
            foreach (DataRow r in dt.Rows) tong += Convert.ToDecimal(r["Thành tiền"]);
        }
        lblTong.Text = "Tổng tiền: " + tong.ToString("N0") + " đ";
    }

    private void Refresh2() { LoadBan(); LoadChiTiet(); }

    private void Run(Action a)
    {
        try { a(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void ThemMon() => Run(() =>
    {
        int maMon = cboMon.SelectedValue is int m ? m : 0;
        HoaDonBUS.ThemMon(maBanChon, maMon, (int)nudSL.Value);
        Refresh2();
    });

    private void ThanhToan() => Run(() =>
    {
        if (MessageBox.Show("Xác nhận thanh toán " + tenBanChon + "?", "Thanh toán", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        decimal tong = HoaDonBUS.ThanhToan(maBanChon);
        MessageBox.Show("Đã thanh toán " + tenBanChon + "\nTổng tiền: " + tong.ToString("N0") + " đ", "Thành công");
        Refresh2();
    });

    private void DoiBan() => Run(() =>
    {
        int moi = cboBanMoi.SelectedValue is int m ? m : 0;
        HoaDonBUS.DoiBan(maBanChon, moi);
        maBanChon = moi;
        tenBanChon = cboBanMoi.Text;
        Refresh2();
    });

    private void MoQuanLyMenu()
    {
        using var f = new MenuManagerForm();
        f.ShowDialog(this);
        LoadMenu();
    }
}
