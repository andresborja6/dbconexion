using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace dbconexion
{
    public partial class Form1 : Form
    {
        public string serverName = "";
        public string databaseName = "";
        public string tableName = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] filePath = new string[0];
            filePath = File.ReadAllLines(Environment.CurrentDirectory + @"\datosserver.txt");
            foreach (string linea in filePath)
            {
                string[] campos = linea.Split(',');
                serverName = campos[0].Trim().Replace("\\\\", "\\");
                databaseName = campos[1].Trim();
                tableName = campos[2].Trim();

            }
            string connectionString = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";Integrated Security=true";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    conexion("cargar", 0, "", "", "", "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Por favor actualice los datos de conexion a la base de datos");
                    Conexion con = new Conexion();
                    this.WindowState = FormWindowState.Minimized;
                    con.Show();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica si se hizo clic en la columna del botón 1
            if (e.ColumnIndex == dataGridView1.Columns["ELIMINAR"].Index && e.RowIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar el registro? " + id, "Confirmar eliminación", MessageBoxButtons.YesNo);
                if (resultado == DialogResult.Yes)
                {
                    conexion("eliminar", id, "", "", "", "");
                    conexion("cargar", 0, "", "", "", "");

                }
            }
            else
            {
                string valor1 = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string valor2 = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string valor3 = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                string valor4 = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                string valor5 = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

                // Asignamos los valores a los TextBox
                txtid.Visible = false;
                lblid.Text = valor1;
                txtapellido.Text = valor2;
                txtnombre.Text = valor3;
                txtdireccion.Text = valor4;
                txtcorreo.Text = valor5;
            }

            // Verifica si se hizo clic en la columna del botón 2

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ELIMINAR")
            {
                DataGridViewButtonCell buttonCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;
                if (buttonCell != null)
                {
                    buttonCell.Style.BackColor = Color.Red;
                }
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "ELIMINAR" && e.RowIndex >= 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                DataGridViewButtonCell celBoton = dataGridView1.Rows[e.RowIndex].Cells["ELIMINAR"] as DataGridViewButtonCell;
                Icon icoAtomico = new Icon(Environment.CurrentDirectory + @"\Cancel_48px.ico");
                e.Graphics.DrawIcon(icoAtomico, e.CellBounds.Left + 20, e.CellBounds.Top + 3);
                dataGridView1.Rows[e.RowIndex].Height = icoAtomico.Height + 5;
                dataGridView1.Columns[e.ColumnIndex].Width = icoAtomico.Width + 40;
                e.Handled = true;
            }
        }
        public void conexion(string evento, int id, string apellido, string nombre, string direccion, string mail)
        {
            string connectionString = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";Integrated Security=true";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                switch (evento)
                {
                    case "cargar":

                        dataGridView1.Columns.Clear();
                        DataTable table = new DataTable();
                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM " + tableName, connection);
                        adapter.Fill(table);

                        dataGridView1.DataSource = table;
                        dataGridView1.Columns[0].HeaderText = "ID";
                        dataGridView1.Columns[1].HeaderText = "APELLIDO";
                        dataGridView1.Columns[2].HeaderText = "NOMBRE";
                        dataGridView1.Columns[3].HeaderText = "DIRECCION";
                        dataGridView1.Columns[4].HeaderText = "E-MAIL";
                        DataGridViewButtonColumn buttonColumn1 = new DataGridViewButtonColumn();
                        buttonColumn1.Name = "ELIMINAR";
                        buttonColumn1.HeaderText = "ELIMINAR";

                        dataGridView1.Columns.Add(buttonColumn1);
                        break;
                    case "modificar":
                        string queryUpdate = "UPDATE " + tableName + " Set apellido = @nuevoApellido, nombre = @nuevoNombre, direccion = @nuevoDireccion, email = @nuevoEmail where idempleado = @idCliente";
                        using (SqlCommand commandUpdate = new SqlCommand(queryUpdate, connection))
                        {
                            commandUpdate.Parameters.AddWithValue("@nuevoApellido", apellido);
                            commandUpdate.Parameters.AddWithValue("@nuevoNombre", nombre);
                            commandUpdate.Parameters.AddWithValue("@nuevoDireccion", direccion);
                            commandUpdate.Parameters.AddWithValue("@nuevoEmail", mail);
                            commandUpdate.Parameters.AddWithValue("@idCliente", id);
                            int rowsAffected = commandUpdate.ExecuteNonQuery();
                            MessageBox.Show($"Registros actualizados: {rowsAffected}");
                        }
                        break;
                    case "insertar":
                        string queryInsert = "Insert into " + tableName + " (idempleado, apellido, nombre, direccion, email) values (@idCliente,@nuevoApellido,@nuevoNombre,@nuevoDireccion,@nuevoEmail)";
                        using (SqlCommand commandInsert = new SqlCommand(queryInsert, connection))
                        {
                            commandInsert.Parameters.AddWithValue("@nuevoApellido", apellido);
                            commandInsert.Parameters.AddWithValue("@nuevoNombre", nombre);
                            commandInsert.Parameters.AddWithValue("@nuevoDireccion", direccion);
                            commandInsert.Parameters.AddWithValue("@nuevoEmail", mail);
                            commandInsert.Parameters.AddWithValue("@idCliente", id);
                            int rowsAffected = commandInsert.ExecuteNonQuery();
                            MessageBox.Show($"Registros Insertados: {rowsAffected}");
                        }
                        break;
                    case "eliminar":
                        string queryDelete = "DELETE FROM " + tableName + " WHERE idempleado = @idCliente";
                        using (SqlCommand commandDelete = new SqlCommand(queryDelete, connection))
                        {
                            commandDelete.Parameters.AddWithValue("@idCliente", id);
                            int rowsAffected = commandDelete.ExecuteNonQuery();
                            MessageBox.Show($"Registros eliminados: {rowsAffected}");
                        }
                        break;
                }

            }

        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            if (txtnombre.Text == "" || txtapellido.Text == "" || txtcorreo.Text == "" || txtdireccion.Text == "")
            {
                MessageBox.Show("Por favor llene todos los datos");
                return;
            }
            else
            {
                if (txtid.Text == "")
                {
                    conexion("modificar", Convert.ToInt32(lblid.Text), txtapellido.Text, txtnombre.Text, txtdireccion.Text, txtcorreo.Text);
                    conexion("cargar", 0, "", "", "", "");
                    limpiar();
                }
                else
                {
                    conexion("insertar", Convert.ToInt32(txtid.Text), txtapellido.Text, txtnombre.Text, txtdireccion.Text, txtcorreo.Text);
                    conexion("cargar", 0, "", "", "", "");
                    limpiar();
                }

            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            limpiar();
        }
        public void limpiar()
        {
            txtid.Visible = true;
            lblid.Text = "Id";
            txtapellido.Text = "";
            txtnombre.Text = "";
            txtdireccion.Text = "";
            txtid.Text = "";
            txtcorreo.Text = "";
        }

        private void txtid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }
    }
}