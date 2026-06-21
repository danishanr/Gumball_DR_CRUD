using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDMahasiswaADO2
{
    public partial class Form2 : Form
    {
        static string connectionString = "Data Source=LAPTOP-49331NDM\\RIANIINDRI;Initial Catalog=DBAkademikADO;Integrated Security=True";

        SqlConnection conn = new SqlConnection(connectionString);
        SqlDataAdapter da;
        DataTable dtMahasiswa;

        RekapMahasiswa listMahasiswa = new RekapMahasiswa();
        string prodi { get; set; }
        DateTime tglmasuk { get; set; }

        public Form2(string Prodi, DateTime TglMasuk)
        {
            InitializeComponent();

            prodi = Prodi;
            tglmasuk = TglMasuk;

            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Report", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inProdi", prodi);
                cmd.Parameters.AddWithValue("@inTglMsuk", tglmasuk.Year);

                da = new SqlDataAdapter(cmd);
                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);
                conn.Close();

                // ✅ Konversi DataTable ke List<Data>
                List<Data> listData = new List<Data>();
                foreach (DataRow row in dtMahasiswa.Rows)
                {
                    listData.Add(new Data()
                    {
                        Nama = row["Nama"].ToString(),
                        JenisKelamin = row["JenisKelamin"].ToString(),
                        Alamat = row["Alamat"].ToString(),
                        NamaProdi = row["NamaProdi"].ToString(),
                        TanggalDaftar = Convert.ToDateTime(row["TanggalDaftar"])
                    });
                }

                listMahasiswa.SetDataSource(listData);
                crystalReportViewer2.ReportSource = listMahasiswa; // ← harus viewer2
                crystalReportViewer2.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void crystalReportViewer2_Load(object sender, EventArgs e)
        {

        }
    }
}
