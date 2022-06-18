using System.Data;

namespace SQLite
{
    public partial class F_GestaoProfessores : Form
    {
        public F_GestaoProfessores()
        {
            InitializeComponent();
        }

        private void F_GestaoProfessores_Load(object sender, EventArgs e)
        {
            string vquery = @"
                SELECT
                    N_IDPROFESSOR as 'ID',
                    T_NOMEPROFESSOR as 'Professor',
                    T_TELEFONE as 'Telefone'
                FROM
                    tb_professores
                ORDER BY
                    N_IDPROFESSOR
            ";
            dgv_professores.DataSource = Banco.DQL(vquery);
            dgv_professores.Columns[0].Width = 40;
            dgv_professores.Columns[1].Width = dgv_professores.Width - 143;
            dgv_professores.Columns[2].Width = 100;
        }

        private void dgv_professores_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            int contLinhas = dgv.SelectedRows.Count;
            if (contLinhas > 0)
            {
                DataTable dt = new();
                string vid = dgv.SelectedRows[0].Cells[0].Value.ToString();
                string vquery = @"
                    SELECT
                        *
                    FROM
                        tb_professores
                    WHERE
                        N_IDPROFESSOR=" + vid;
                dt = Banco.DQL(vquery);
                tb_id.Text = dt.Rows[0].Field<Int64>("N_IDPROFESSOR").ToString();
                tb_professor.Text = dt.Rows[0].Field<string>("T_NOMEPROFESSOR");
                mtb_telefone.Text = dt.Rows[0].Field<string>("T_TELEFONE");

            }
        }

        private void btn_salvar_Click(object sender, EventArgs e)
        {
            string vquery;
            if(tb_id.Text == "")
            {
                vquery = "INSERT INTO tb_professores (T_NOMEPROFESSOR, T_TELEFONE) VALUES ('" + tb_professor.Text + "','" + mtb_telefone.Text + "')";
            }
            else
            {
                vquery = "UPDATE tb_professores SET T_NOMEPROFESSOR='" + tb_professor.Text + "', T_TELEFONE='" + mtb_telefone.Text + "' WHERE N_IDPROFESSOR=" + tb_id.Text;
            }
            Banco.DML(vquery);
            vquery = @"
                SELECT
                    N_IDPROFESSOR as 'ID',
                    T_NOMEPROFESSOR as 'Professor',
                    T_TELEFONE as 'Telefone',
                FROM
                    tb_professores
                ORDER BY
                    N_IDPROFESSOR
            ";
            dgv_professores.DataSource = Banco.DQL(vquery);
        }

        private void btn_novo_Click(object sender, EventArgs e)
        {
            tb_id.Clear();
            tb_professor.Clear();
            mtb_telefone.Clear();
            tb_professor.Focus();
        }

        private void btn_excluir_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Confirma exclusão?", "Excluir?", MessageBoxButtons.YesNo);
            if (res == DialogResult.No) return;
            string vquery = "DELETE FROM tb_professores WHERE N_IDPROFESSOR=" + tb_id.Text;
            Banco.DML(vquery);
            dgv_professores.Rows.Remove(dgv_professores.CurrentRow);
        }

        private void btn_fechar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
