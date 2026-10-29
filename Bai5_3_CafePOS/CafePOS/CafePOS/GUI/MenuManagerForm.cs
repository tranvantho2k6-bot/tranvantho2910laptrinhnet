using System.Data;
using CafePOS.BUS;
using CafePOS.DTO;
namespace CafePOS.GUI;

public class MenuManagerForm : Form
{
    private DataGridView dgvDM, dgvMon;
    private TextBox txtDM, txtTen, txtGia;
    private ComboBox cboDM;
    private bool loading;

    public MenuManagerForm()
    {
        Text = "Quản lý danh mục & thực đơn";
        ClientSize = new Size(1100, 530);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        Ui.AddLabel(this, "DANH MỤC", 10, 8).Font = new Font("Segoe UI", 10, FontStyle.Bold);
        dgvDM = Ui.AddGrid(this, 10, 35, 400, 340);
        Ui.AddLabel(this, "Tên danh mục", 10, 395);
        txtDM = Ui.AddText(this, 110, 392, 300);
        Ui.AddButton(this, "Thêm", 10, 435, 120, (s, e) => Run(() => MenuBUS.ThemDanhMuc(txtDM.Text)));
        Ui.AddButton(this, "Sửa", 140, 435, 120, (s, e) => Run(() => MenuBUS.SuaDanhMuc(SelectedId(dgvDM, "MaDanhMuc"), txtDM.Text)));
        Ui.AddButton(this, "Xóa", 270, 435, 120, (s, e) => Run(() => { if (Confirm()) MenuBUS.XoaDanhMuc(SelectedId(dgvDM, "MaDanhMuc")); }));

        Ui.AddLabel(this, "THỰC ĐƠN", 430, 8).Font = new Font("Segoe UI", 10, FontStyle.Bold);
        dgvMon = Ui.AddGrid(this, 430, 35, 660, 340);
        Ui.AddLabel(this, "Tên món", 430, 395);
        txtTen = Ui.AddText(this, 495, 392, 180);
        Ui.AddLabel(this, "Danh mục", 690, 395);
        cboDM = Ui.AddCombo(this, 760, 392, 150);
        Ui.AddLabel(this, "Đơn giá", 925, 395);
        txtGia = Ui.AddText(this, 985, 392, 100);
        Ui.AddButton(this, "Thêm món", 430, 435, 130, (s, e) => Run(() => MenuBUS.ThemMon(ReadMon(0))));
        Ui.AddButton(this, "Sửa món / giá", 570, 435, 130, (s, e) => Run(() => MenuBUS.SuaMon(ReadMon(SelectedId(dgvMon, "MaMon")))));
        Ui.AddButton(this, "Xóa món", 710, 435, 130, (s, e) => Run(() => { if (Confirm()) MenuBUS.XoaMon(SelectedId(dgvMon, "MaMon")); }));

        dgvDM.SelectionChanged += (s, e) =>
        {
            if (!loading && dgvDM.CurrentRow?.DataBoundItem is DataRowView v) txtDM.Text = v["TenDanhMuc"].ToString();
        };
        dgvMon.SelectionChanged += (s, e) =>
        {
            if (loading || dgvMon.CurrentRow?.DataBoundItem is not DataRowView v) return;
            txtTen.Text = v["TenMon"].ToString();
            txtGia.Text = Convert.ToDecimal(v["DonGia"]).ToString("0.##");
            cboDM.SelectedValue = v["MaDanhMuc"];
        };
        Load += (s, e) => { try { LoadAll(); } catch (Exception ex) { MessageBox.Show("Không tải được dữ liệu từ CSDL.\n\nHãy kiểm tra: (1) đã bật MySQL trong XAMPP Control Panel chưa, (2) đã import Database/ScriptDB.sql bằng phpMyAdmin chưa, (3) chuỗi kết nối trong App.config (Server, Port, User ID, Password) có đúng không.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi kết nối CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error); } };
    }

    private void LoadAll()
    {
        loading = true;
        dgvDM.DataSource = MenuBUS.GetDanhMuc();
        dgvMon.DataSource = MenuBUS.GetMonAn();
        cboDM.DisplayMember = "TenDanhMuc";
        cboDM.ValueMember = "MaDanhMuc";
        cboDM.DataSource = MenuBUS.GetDanhMuc();
        dgvDM.Columns["MaDanhMuc"].Visible = false;
        dgvMon.Columns["MaMon"].Visible = false;
        dgvMon.Columns["MaDanhMuc"].Visible = false;
        dgvMon.Columns["DonGia"].DefaultCellStyle.Format = "N0";
        loading = false;
    }

    private MonAnDTO ReadMon(int ma)
    {
        if (!decimal.TryParse(txtGia.Text.Trim(), out var gia))
            throw new ArgumentException("Đơn giá phải là một số hợp lệ.");
        return new MonAnDTO
        {
            MaMon = ma,
            TenMon = txtTen.Text,
            MaDanhMuc = cboDM.SelectedValue is int d ? d : 0,
            DonGia = gia
        };
    }

    private static int SelectedId(DataGridView g, string col) =>
        g.CurrentRow?.DataBoundItem is DataRowView v ? Convert.ToInt32(v[col]) : 0;

    private bool Confirm() =>
        MessageBox.Show("Bạn chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes;

    private void Run(Action a)
    {
        try { a(); LoadAll(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}
