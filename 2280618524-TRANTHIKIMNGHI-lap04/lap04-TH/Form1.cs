
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
    public partial class Form1 : Form
    {
        Model1 context = new Model1();
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            rdbNu.Checked = true;

            txtTSVN.Text = "0";
            txtTSVNu.Text = "0";

            txtTSVN.Enabled = false;
            txtTSVNu.Enabled = false;

            try
            {
                List<Faculty> listFaculty = context.Faculties.ToList();
                List<Student> listStudent = context.Students.ToList();
                FillFalcultyCombobox(listFaculty);
                BindGrid(listStudent);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private bool ValidateInput()
        {

            if (string.IsNullOrWhiteSpace(txtMaSV.Text) ||
                string.IsNullOrWhiteSpace(txtHT.Text) ||
                string.IsNullOrWhiteSpace(txtDTB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                
                return false;
            }

            if (txtMaSV.Text.Length != 10 || !txtMaSV.Text.All(char.IsDigit))
            {
                MessageBox.Show( "Mã số sinh viên không hợp lệ.","Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false; 

            }

            if (!decimal.TryParse(txtDTB.Text, out decimal avgScore) || avgScore < 0 || avgScore > 10)
            {
                MessageBox.Show("Điểm trung bình sinh viên không hợp lệ.","Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false; 

            }

            if (txtHT.Text.Length < 3 || txtHT.Text.Length > 100 ||
                !txtHT.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                MessageBox.Show("Tên sinh viên không hợp lệ.","Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;

            }
            return true;
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
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore;
                dgvStudent.Rows[index].Cells[4].Value = item.Faculty.FacultyName;
            }
        }
        private void CountMaleFemaleStudents()
        {
            
            List<Student> students = context.Students.ToList();

            
            int maleCount = students.Count(s => s.Gender == "Male");
            int femaleCount = students.Count(s => s.Gender == "Female");

           
            txtTSVN.Text = maleCount.ToString();
            txtTSVNu.Text = femaleCount.ToString();
        }

        private void dgvStudent_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selected = dgvStudent.Rows[e.RowIndex];
                txtMaSV.Text = selected.Cells[0].Value.ToString();
                txtHT.Text = selected.Cells[1].Value.ToString();
                string gender = selected.Cells[2].Value.ToString();
                if (gender == "Male")
                {
                    rdbNam.Checked = true;
                }
                else
                {
                    rdbNu.Checked = true;
                }
                txtDTB.Text = selected.Cells[3].Value.ToString();
                cmbFaculty.Text = selected.Cells[4].Value.ToString();
            }
        }

        private void Reset()
        {
            txtMaSV.Clear();
            txtHT.Clear();
            rdbNam.Checked = false; rdbNu.Checked = false;
            cmbFaculty.SelectedIndex = 0;
            txtDTB.Clear();
            dgvStudent.ClearSelection();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
               if( !ValidateInput())
                    return;

                List<Student> students = context.Students.ToList();
                if (students.Any(s => s.StudentID == txtMaSV.Text))
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại. Vui lòng nhập một mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var newStudent = new Student
                {
                    StudentID = txtMaSV.Text,
                    FullName = txtHT.Text,
                    Gender = rdbNam.Checked ? "Male" : "Female",
                    FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString()),
                    AverageScore = double.Parse(txtDTB.Text)
                };
                context.Students.Add(newStudent);
                context.SaveChanges();
                RefreshData();
                Reset();

                MessageBox.Show("Thêm sinh viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {

            try
            {
                if(!ValidateInput())
                    return;

                var student = context.Students.FirstOrDefault(s => s.StudentID == txtMaSV.Text);
                if (student != null)
                {
                    student.FullName = txtHT.Text;
                    student.Gender = rdbNam.Checked ? "Male" : "Female";
                    student.FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString());
                    student.AverageScore = double.Parse(txtDTB.Text);

                    context.SaveChanges();
                    RefreshData();
                    Reset();

                    MessageBox.Show("Sửa sinh viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sinh viên không tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sửa dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                var student = context.Students.FirstOrDefault(s => s.StudentID == txtMaSV.Text);

                if (student != null)
                {
                    DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa sinh viên này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        context.Students.Remove(student);
                        context.SaveChanges();
                        RefreshData();
                        Reset();

                        MessageBox.Show("Xóa sinh viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Sinh viên không tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Thông Báo",MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                this.Close();
            //DialogResult result = MessageBox.Show("Bạn có chắc muốn thoát chương trình?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //if (result == DialogResult.Yes)
            //{
            //    Application.Exit(); 
            //}
        }

        private void RefreshData()
        {
            try
            {
                List<Student> listStudent = context.Students.ToList();
                BindGrid(listStudent);
                CountMaleFemaleStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi làm mới dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }
    }
}