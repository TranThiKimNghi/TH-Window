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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        private void ResetInputFields()
        {
            txtMaSV.Clear();
            txtHT.Clear();
            cmbFaculty.SelectedIndex = -1;
        }
        private void FillFalcultyCombobox(List<Faculty> listFalcultys)
        {
            this.cmbFaculty.DataSource = listFalcultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }
        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                dgvStudent.Rows[index].Cells[2].Value = item.Gender;
                dgvStudent.Rows[index].Cells[3].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[4].Value = item.AverageScore;
            }
        }
  
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
               
                DataGridViewRow row = dgvStudent.Rows[e.RowIndex];
                txtMaSV.Text = row.Cells[0].Value.ToString();
                txtHT.Text = row.Cells[1].Value.ToString();
                if (row.Cells[2].Value.ToString() == "Nam")
                {
                   rdbNam.Checked = true;
                }
                else
                {
                    rdbNu.Checked = true;
                }
                cmbFaculty.Text = row.Cells[4].Value.ToString();
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<Faculty> listFalcultys = context.Faculties.ToList();
                List<Student> listStudent = context.Students.ToList();
                FillFalcultyCombobox(listFalcultys);
                BindGrid(listStudent);
                ResetInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnTK_Click(object sender, EventArgs e)
        {
            string gender = rdbNu.Checked ? "Nữ" : "Nam";
            string facultyID = cmbFaculty.SelectedValue != null ? cmbFaculty.SelectedValue.ToString() : null;

            Model1 context = new Model1();
            var query = context.Students.AsQueryable();
            // Áp dụng điều kiện tìm kiếm nếu Mã SV không trống
            if (!string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                query = query.Where(s => s.StudentID.ToLower().Contains(txtMaSV.Text));
            }

            // Áp dụng điều kiện tìm kiếm nếu Họ tên không trống
            if (!string.IsNullOrWhiteSpace(txtHT.Text))
            {
                query = query.Where(s => s.FullName.ToLower().Contains(txtHT.Text));
            }

            // Áp dụng điều kiện tìm kiếm theo Giới tính nếu người dùng có chọn
            if (gender != null)
            {
                query = query.Where(s => s.Gender == gender);
            }

            // Áp dụng điều kiện tìm kiếm theo Khoa nếu có chọn khoa
            if (!string.IsNullOrWhiteSpace(facultyID) && int.TryParse(facultyID, out int faculty))
            {
                query = query.Where(s => s.FacultyID == faculty);
            }


            // Thực hiện truy vấn
            var results = query.ToList();

            // Hiển thị kết quả
            if (results.Count == 0)
            {
                MessageBox.Show("Không tìm thấy kết quả.");
                txtTK.Text = "0"; // Hiển thị số lượng kết quả tìm kiếm
            }
            else
            {
                txtTK.Text = results.Count.ToString(); // Hiển thị số lượng kết quả tìm kiếm
                dgvStudent.Rows.Clear();
                foreach (var student in results)
                {
                    // Hiển thị các thông tin sinh viên (bao gồm Khoa và Điểm trung bình)
                    dgvStudent.Rows.Add(student.StudentID, student.FullName, student.Gender, student.AverageScore, student.Faculty.FacultyName);
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            txtMaSV.Clear();
            txtHT.Clear();
            cmbFaculty.SelectedIndex = -1;
            rdbNam.Checked = false;
            rdbNu.Checked = false;
            dgvStudent.Rows.Clear();
        }

        private void btnTroVe_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
