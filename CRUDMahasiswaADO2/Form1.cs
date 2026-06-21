using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 
namespace CRUDMahasiswaADO2
{
    public partial class Form1 : Form
    {
        private readonly SqlConnection conn;
        private readonly string connectionString = "Data Source=LAPTOP-49331NDM\\RIANIINDRI;Initial Catalog=DBAkademikADO;Integrated Security=True";

        private void SimpanLog(string pesan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO LogError
                                 VALUES(GETDATE(), @pesan)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pesan", pesan);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Tambahan Prak 9
        private BindingSource bindingSource = new BindingSource();
        private DataTable dtMahasiswa = new DataTable();    

        public Form1()
        {
            InitializeComponent();
            conn  = new SqlConnection(connectionString);

        }

        private void label6_Click(object sender, EventArgs e)
        {


        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {

            SqlConnection conn =
                new SqlConnection(connectionString);

            conn.Open();

            SqlTransaction trans =
                conn.BeginTransaction();

            try
            {
                SqlCommand cmd =
                    new SqlCommand(
                    "sp_InsertMahasiswa",
                    conn,
                    trans);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@NIM",
                    txtNIM.Text);

                cmd.Parameters.AddWithValue(
                    "@Nama",
                    txtNama.Text);

                cmd.Parameters.AddWithValue(
                    "@JenisKelamin",
                    cmbJK.Text);

                cmd.Parameters.AddWithValue(
                    "@TanggalLahir",
                     dtpTanggalLahir.Value.Date);

                cmd.Parameters.AddWithValue(
                    "@Alamat",
                    txtAlamat.Text);

                cmd.Parameters.AddWithValue(
                    "@KodeProdi",
                    txtKodeProdi.Text);

                cmd.Parameters.AddWithValue(
                    "@TanggalDaftar",
                    DateTime.Now);

                cmd.ExecuteNonQuery();

                SqlCommand cmdLog =
                    new SqlCommand(
                    @"INSERT INTO LogAktivitasSalah
            (aktivitas,waktu)
            VALUES
            (@aktivitas,GETDATE())",
                    conn,
                    trans);

                cmdLog.Parameters.AddWithValue(
                    "@aktivitas",
                    "INSERT MAHASISWA : " +
                    txtNIM.Text);

                cmdLog.ExecuteNonQuery();

                trans.Commit();

                MessageBox.Show(
                    "Data berhasil ditambahkan");

                LoadData();
            }

            catch (SqlException ex)
            {
                trans.Rollback();

                SimpanLog(
                    "ROLLABCK INSERT : " +
                    ex.Message);

                MessageBox.Show(
                    ex.Message);

            }
            catch (Exception ex)
            {
                trans.Rollback();

                SimpanLog(
                    "GENERAL ERROR : " +
                    ex.Message);

                MessageBox.Show(
                    ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            // Tambahan Prak 9 - Connect Test
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    MessageBox.Show("Koneksi berhasil");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand("sp_UpdateMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@NIM", txtNIM.Text);
                        cmd.Parameters.AddWithValue("@Nama", txtNama.Text);

                        cmd.Parameters.AddWithValue(
                            "@JenisKelamin",
                            cmbJK.Text);

                        cmd.Parameters.AddWithValue(
                            "@TanggalLahir",
                            dtpTanggalLahir.Value.Date);

                        cmd.Parameters.AddWithValue(
                            "@Alamat",
                            txtAlamat.Text);

                        cmd.Parameters.AddWithValue(
                            "@KodeProdi",
                            txtKodeProdi.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil diupdate");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand("sp_DeleteMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@NIM", txtNIM.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            MessageBox.Show("Data tidak ditemukan");
                        else
                            MessageBox.Show("Data berhasil dihapus");
                    }
                }

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ConnectDatabase()
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                MessageBox.Show("koneksi berhasil");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtNIM.Text = row.Cells["NIM"].Value.ToString();
                txtNama.Text = row.Cells["Nama"].Value.ToString();
                cmbJK.Text = row.Cells["JenisKelamin"].Value.ToString();
                dtpTanggalLahir.Value = Convert.ToDateTime(row.Cells["TanggalLahir"].Value);
                txtAlamat.Text = row.Cells["Alamat"].Value.ToString();
                txtKodeProdi.Text = row.Cells["KodeProdi"].Value.ToString();
            }
        }

        private void ClearForm()
        {
            txtNIM.Clear();
            txtNama.Clear();
            cmbJK.SelectedIndex = -1;
            txtAlamat.Clear();
            txtKodeProdi.Clear();
            dtpTanggalLahir.Value = DateTime.Now;
            txtNIM.Focus();
        }

        //Tambahan Prak 9
        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        dtMahasiswa = new DataTable();
                        da.Fill(dtMahasiswa);

                        bindingSource.DataSource = dtMahasiswa; 
                        dataGridView1.DataSource = dtMahasiswa;

                        BindControls();
                    }
                }
            }

            HitungTotal(); // tambahkan ini
        }

        // Tambahan Prak 9
        private void BindControls()
        {
            txtNIM.DataBindings.Clear();
            txtNama.DataBindings.Clear();   
            cmbJK.DataBindings.Clear();
            dtpTanggalLahir.DataBindings.Clear();
            txtAlamat.DataBindings.Clear();    
            txtKodeProdi.DataBindings.Clear();

            txtNIM.DataBindings.Add("Text", bindingSource, "NIM");
            txtNama.DataBindings.Add("Text", bindingSource, "Nama");
            cmbJK.DataBindings.Add("Text", bindingSource, "JenisKelamin");
            dtpTanggalLahir.DataBindings.Add("Value", bindingSource, "TanggalLahir");
            txtAlamat.DataBindings.Add("Text", bindingSource, "Alamat");
            txtKodeProdi.DataBindings.Add("Text", bindingSource, "KodeProdi");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                bindingSource.Position = e.RowIndex;
            }
        }

        private void btnResetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        IF OBJECT_ID('dbo.Mahasiswa_Backup') IS NOT NULL
                        BEGIN
                            DELETE FROM dbo.Mahasiswa;
                            INSERT INTO dbo.Mahasiswa
                            SELECT * FROM dbo.Mahasiswa_Backup;
                        END";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil direset");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reser gagal: " + ex.Message);
            }
        }

        private void btnTestInjection_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                {
                    string query =
                        "UPDATE Mahasiswa SET Nama='" +
                        txtNama.Text +
                        "' WHERE NIM='" +
                        txtNIM.Text + "'";

                    SqlCommand cmd =
                    new SqlCommand(query, conn);

                    conn.Open();

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Update berhasil");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            cmbJK.DataSource = new string[] { "L", "P" };

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Tambahan Prak 9
            bindingNavigator1.BindingSource = bindingSource;

            // Tambahan Prak 9
            LoadData();
        }

        private void HitungTotal()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
                        outputParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParam);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        lblTotal.Text = "Total Mahasiswa: " + outputParam.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gogal menghitung total: " + ex.Message);
            }
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            FormReportPrak13 fm3 = new FormReportPrak13();
            fm3.Show();
            this.Hide();
        }
    }
}