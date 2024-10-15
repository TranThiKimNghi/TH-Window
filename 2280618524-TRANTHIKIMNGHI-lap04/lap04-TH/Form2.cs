using lap04_TH.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lap04_TH
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private bool ValidateInput()
        {


            if (string.IsNullOrWhiteSpace(txtMK.Text) ||
                string.IsNullOrWhiteSpace(txtTK.Text) ||
                string.IsNullOrWhiteSpace(txtTSGS.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtMK.Text, out int FacultyID) || FacultyID < 0 || FacultyID > 1000)
            {
                MessageBox.Show("Mã khoa không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtTSGS.Text, out int TotalTeachers) || TotalTeachers < 0 || TotalTeachers > 1000)
            {
                MessageBox.Show("Tổng GS không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtTK.Text.Length < 3 || txtTK.Text.Length > 100 ||
                !txtTK.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                MessageBox.Show("Tên khoa không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        private void BindGrid(List<Faculty> listFalcultys)
        {
            dgvFaculty.Rows.Clear();
            foreach (var item in listFalcultys)
            {
                int index = dgvFaculty.Rows.Add();
                dgvFaculty.Rows[index].Cells[0].Value = item.FacultyID;
                dgvFaculty.Rows[index].Cells[1].Value = item.FacultyName;
                dgvFaculty.Rows[index].Cells[2].Value = item.TotalProfessor;
            }
        }

        private void Sort()
        {
            bool isAscending = cmbSX.SelectedItem != null && cmbSX.SelectedItem.ToString() == "Tăng dần";

            if (isAscending)
            {
                dgvFaculty.Sort(dgvFaculty.Columns[2], ListSortDirection.Ascending);
            }
            else
            {
                dgvFaculty.Sort(dgvFaculty.Columns[2], ListSortDirection.Descending);
            }
        }
        private void UpdateTeacherCounts()
        {
            try
            {
                // Kết nối tới database
                Model1 context = new Model1();

                // Lấy danh sách các khoa từ cơ sở dữ liệu
                var danhSachKhoa = context.Faculties.ToList();

                // Tính tổng số Giáo Sư
                int tongSoGS = danhSachKhoa.Sum(khoa => khoa.TotalProfessor ?? 0);

                // Hiển thị tổng số Giáo Sư lên TextBox
                txtTS.Text = tongSoGS.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi tính tổng số Giáo Sư: " + ex.Message);
            }
        }

        private void ResetInputFields()
        {
            txtMK.Clear();
            txtTK.Clear();
            txtTSGS.Clear();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            Model1 context = new Model1();
            List<Faculty> listFalcultys = context.Faculties.ToList();
            BindGrid(listFalcultys);
            UpdateTeacherCounts();
            cmbSX.Items.Add("Tăng dần");
            cmbSX.Items.Add("Giảm dần");

            txtTS.Text = "0";
            txtTS.Enabled = false;
        }

        private void cmbSX_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            Sort();
        }

        private void dgvFaculty_SortCompare_1(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2)
            {
                // Chuyển đổi giá trị từ chuỗi sang kiểu số (float hoặc double)
                float value1 = float.Parse(e.CellValue1.ToString());
                float value2 = float.Parse(e.CellValue2.ToString());

                // So sánh hai giá trị số
                e.SortResult = value1.CompareTo(value2);
            }
            else
            {
                // Sử dụng cách so sánh mặc định cho các cột khác
                e.SortResult = string.Compare(e.CellValue1.ToString(), e.CellValue2.ToString());
            }

            e.Handled = true;

        }

        private void btnThem_Click(object sender, EventArgs e)
        {

            if (!ValidateInput()) return;

            try
            {
                Model1 context = new Model1();

                // Create new faculty object
                Faculty newFaculty = new Faculty
                {
                    FacultyID = int.Parse(txtMK.Text),
                    FacultyName = txtTK.Text,
                    TotalProfessor = int.Parse(txtTSGS.Text)
                };

                // Add new student to the database
                context.Faculties.Add(newFaculty);
                context.SaveChanges();

                // Add new student to DataGridView
                dgvFaculty.Rows.Add(newFaculty.FacultyID, newFaculty.FacultyName, newFaculty.TotalProfessor);

                MessageBox.Show("Thêm mới dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            MessageBox.Show("Mã khoa đã tồn tại.");
        

            ResetInputFields();
            UpdateTeacherCounts();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                Model1 context = new Model1();
                var faculty = context.Faculties.FirstOrDefault(f => f.FacultyID.ToString() == txtMK.Text);

                if (faculty != null)
                {
                    faculty.FacultyName = txtTK.Text;
                    faculty.TotalProfessor = int.Parse(txtTSGS.Text);

                    context.SaveChanges();

                    // Update DataGridView
                    var row = dgvFaculty.Rows.Cast<DataGridViewRow>()
                                             .FirstOrDefault(r => r.Cells[0].Value.ToString() == txtMK.Text);
                    if (row != null)
                    {
                        row.Cells[1].Value = faculty.FacultyName;
                        row.Cells[2].Value = faculty.TotalProfessor;
                    }

                    MessageBox.Show("Sửa dữ liệu thành công!");
                }
                else
                {
                    MessageBox.Show("Khoa không tồn tại.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            txtMK.Enabled = true;
            btnThem.Enabled = true;
            ResetInputFields();
            UpdateTeacherCounts();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMK.Text))
            {
                MessageBox.Show("Vui lòng chọn khoa cần xóa.");
                return;
            }
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa khoa này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    Model1 context = new Model1();
                    var faculty = context.Faculties.FirstOrDefault(f => f.FacultyID.ToString() == txtMK.Text);

                    if (faculty != null)
                    {
                        context.Faculties.Remove(faculty);
                        context.SaveChanges();

                        // Remove from DataGridView
                        var row = dgvFaculty.Rows.Cast<DataGridViewRow>()
                                                 .FirstOrDefault(r => r.Cells[0].Value.ToString() == txtMK.Text);
                        if (row != null)
                        {
                            dgvFaculty.Rows.Remove(row);
                        }

                        MessageBox.Show("Xóa dữ liệu thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Sinh viên không tồn tại.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            txtMK.Enabled = true;
            btnThem.Enabled = true;
            ResetInputFields();
            UpdateTeacherCounts();
        }

        private void btnDong_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Bạn có chắc muốn đóng  chương trình?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void dgvFaculty_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                // Load data from DataGridView to input fields
                DataGridViewRow row = dgvFaculty.Rows[e.RowIndex];
                txtMK.Text = row.Cells[0].Value.ToString();
                txtTK.Text = row.Cells[1].Value.ToString();
                txtTSGS.Text = row.Cells[2].Value.ToString();

                txtMK.Enabled = false;
                btnThem.Enabled = false;

            }
        }
    }
}

