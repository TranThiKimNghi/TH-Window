using lap04_TH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace lap04_TH
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Giới tính "Nữ" được chọn mặc định
            rdbNu.Checked = true;

            // TextBox hiển thị kết quả tìm kiếm vô hiệu hóa
            txtTK.Text = "0";
            txtTK.Enabled = false;

            try
            {
                Model1 context = new Model1();
                List<Faculty> listFaculties = context.Faculties.ToList();
                List<Student> listStudents = context.Students.ToList();

                // Điền dữ liệu vào ComboBox Khoa
                FillFalcultyCombobox(listFaculties);

                // Hiển thị danh sách sinh viên có trong hệ thống ban đầu
                BindGrid(listStudents);

                ResetInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillFalcultyCombobox(List<Faculty> listFaculties)
        {
            cmbFaculty.DataSource = listFaculties;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudents)
        {
            dgvStudent.Rows.Clear();
            foreach (var student in listStudents)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = student.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = student.FullName;
                dgvStudent.Rows[index].Cells[2].Value = student.Gender;
                dgvStudent.Rows[index].Cells[3].Value = student.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[4].Value = student.AverageScore;
            }
        }

        // Reset các trường nhập liệu
        private void ResetInputFields()
        {
            txtMaSV.Clear();
            txtHT.Clear();
            cmbFaculty.SelectedIndex = -1;
            rdbNam.Checked = false;
            rdbNu.Checked = true;
        }

        // Chức năng Tìm kiếm
        private void btnTK_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo context để truy xuất CSDL
                Model1 context = new Model1();

                // Lấy danh sách sinh viên hiện tại từ CSDL
                List<Student> students = context.Students.ToList();

                // Áp dụng điều kiện tìm kiếm theo Mã sinh viên
                if (!string.IsNullOrWhiteSpace(txtMaSV.Text))
                {
                    students = students.Where(s => s.StudentID.Contains(txtMaSV.Text)).ToList();
                }

                // Áp dụng điều kiện tìm kiếm theo Họ tên
                if (!string.IsNullOrWhiteSpace(txtHT.Text))
                {
                    students = students.Where(s => s.FullName.ToLower().Contains(txtHT.Text.ToLower())).ToList();
                }

                // Áp dụng điều kiện tìm kiếm theo Giới tính
                if (rdbNam.Checked)
                {
                    students = students.Where(s => s.Gender == "Male").ToList();
                }
                else if (rdbNu.Checked)
                {
                    students = students.Where(s => s.Gender == "Female").ToList();
                }

                // Áp dụng điều kiện tìm kiếm theo Khoa
                if (cmbFaculty.SelectedIndex != -1)
                {
                    int selectedFacultyId = (int)cmbFaculty.SelectedValue;
                    students = students.Where(s => s.FacultyID == selectedFacultyId).ToList();
                }

                // Kiểm tra kết quả tìm kiếm
                if (students.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy kết quả.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtTK.Text = "0"; // Cập nhật số lượng kết quả tìm thấy
                    dgvStudent.Rows.Clear(); // Xóa dữ liệu cũ từ DataGridView
                }
                else
                {
                    // Cập nhật số lượng kết quả tìm thấy
                    txtTK.Text = students.Count.ToString();

                    // Hiển thị kết quả tìm kiếm lên DataGridView
                    BindGrid(students);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        // Chức năng Xóa Khoa
        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
               
                Model1 context = new Model1();

                // Kiểm tra mã khoa có được chọn trong ComboBox không
                if (cmbFaculty.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng chọn mã khoa để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                int selectedFacultyId = (int)cmbFaculty.SelectedValue;

               
                Faculty facultyToDelete = context.Faculties.FirstOrDefault(f => f.FacultyID == selectedFacultyId);

               
                if (facultyToDelete == null)
                {
                    MessageBox.Show("Mã khoa không tồn tại trong hệ thống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

               
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa khoa này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                   
                    context.Faculties.Remove(facultyToDelete);
                    context.SaveChanges();
                    MessageBox.Show("Xóa khoa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetInputFields();

                    
                    List<Faculty> listFaculties = context.Faculties.ToList();
                    FillFalcultyCombobox(listFaculties); 
                    BindGrid(context.Students.ToList()); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa khoa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Chức năng Trở về
        private void btnTroVe_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn trở về màn hình chính?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
