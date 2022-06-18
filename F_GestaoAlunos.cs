using System.Data;

namespace SQLite
{
    public partial class F_GestaoAlunos : Form
    {
        string vqueryDGV = "";
        string turmaAtual = "";
        string idSelecionado = "";
        string turma = "";
        int linha = 0;

        public F_GestaoAlunos()
        {
            InitializeComponent();
        }

        private void F_GestaoAlunos_Load(object sender, EventArgs e)
        {
            vqueryDGV = @"
                SELECT
                    N_IDALUNO as 'ID',
                    T_NOMEALUNO as 'Aluno'
                FROM
                    tb_alunos
            ";
            dgv_alunos.DataSource = Banco.DQL(vqueryDGV);
            dgv_alunos.Columns[0].Width = 40;
            dgv_alunos.Columns[1].Width = dgv_alunos.Width - dgv_alunos.Columns[0].Width - 3;

            tb_nome.Text = dgv_alunos.Rows[dgv_alunos.SelectedRows[0].Index].Cells[1].Value.ToString();

            //popular combobox turmas
            string vqueryTurmas = @"
                SELECT
                    N_IDTURMA,
                    ('Vagas: '|| (
                                   (N_MAXALUNOS)-(
                                                    SELECT
                                                        count(tba.N_IDALUNO)
                                                    FROM
                                                        tb_alunos as tba
                                                    WHERE
                                                        tba.T_STATUS='A' and tba.N_IDTURMA
                                                 )
                                 ) || ' / Turma: ' || T_DSCTURMA
                    )as 'Turma'
                FROM
                    tb_turmas
                ORDER BY
                    N_IDTURMA
            ";
            cb_turmas.Items.Clear();
            cb_turmas.DataSource = Banco.DQL(vqueryTurmas);
            cb_turmas.DisplayMember = "Turma";
            cb_turmas.ValueMember = "N_IDTURMA";

            //Popular combobox status ("tivo=A, bloqueado=B, cancelado=C)
            Dictionary<string, string> status = new Dictionary<string, string>();
            status.Add("A", "Ativo");
            status.Add("B", "Bloqueado");
            status.Add("V", "Cancelado");
            cb_status.DataSource = new BindingSource(status, null);
            cb_status.DisplayMember = "Value";
            cb_status.ValueMember = "Key";

            turma = cb_turmas.Text;
            turmaAtual = cb_turmas.Text;
            idSelecionado = dgv_alunos.Rows[0].Cells[0].Value.ToString();
        }

        private void btn_salvar_Click(object sender, EventArgs e)
        {
            turma = cb_turmas.Text;
            if(turmaAtual != turma)
            {
                string[] t = turma.Split(' ');
                int vagas = int.Parse(t[1]);
                if (vagas < 1)
                {
                    MessageBox.Show("Não há vagas na turma selecionada, selecione outra turma");
                    cb_turmas.Focus();
                    return;
                }
            }
            linha = dgv_alunos.SelectedRows[0].Index;
            string queryAtualizarAluno = string.Format(@"
                UPDATE
                    tb_alunos
                SET
                    T_NOMEALUNO='{0}',
                    T_TELEFONE='{1}',
                    T_STATUS='{2}',
                    N_IDTURMA='{3}'
                WHERE
                    N_IDALUNO={4}
            ",tb_nome.Text,mtb_telefone.Text,cb_status.SelectedValue,cb_turmas.SelectedValue,idSelecionado);
            Banco.DML(queryAtualizarAluno);
            dgv_alunos[1, linha].Value = tb_nome.Text;

        }

        private void btn_excluir_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Confirma exclusão?", "Excluir", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (File.Exists(pictureBox1.ImageLocation)) File.Delete(pictureBox1.ImageLocation);
                string vqueryExcluirAluno = String.Format(@"
                    DELETE FROM
                        tb_alunos
                    WHERE
                        N_IDALUNO={0}
                ",idSelecionado);   
                Banco.DML(vqueryExcluirAluno);
                dgv_alunos.Rows.Remove(dgv_alunos.CurrentRow);
                tb_nome.Clear();
                mtb_telefone.Clear();
                cb_status.Text = "";
                cb_turmas.Text = "";
                //img box remover tbm;

            }
        }
        private void btn_fechar_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void dgv_alunos_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if(dgv.SelectedRows.Count > 0)
            {
                idSelecionado = dgv_alunos.Rows[0].Cells[0].Value.ToString();
                tb_nome.Text = dgv_alunos.Rows[dgv_alunos.SelectedRows[0].Index].Cells[1].Value.ToString();
                idSelecionado = dgv.Rows[dgv.SelectedRows[0].Index].Cells[0].Value.ToString();
                string vqueryCampos = string.Format(@"
                    SELECT
                        N_IDALUNO,
                        T_NOMEALUNO,
                        T_TELEFONE,
                        T_STATUS,
                        N_IDTURMA,
                        T_FOTO
                    FROM
                        tb_alunos
                    WHERE N_IDALUNO={0}
                ",idSelecionado);
                DataTable dt = Banco.DQL(vqueryCampos);
                tb_nome.Text = dt.Rows[0].Field<string>("T_NOMEALUNO");
                mtb_telefone.Text = dt.Rows[0].Field<string>("T_TELEFONE");
                cb_status.SelectedValue = dt.Rows[0].Field<string>("T_STATUS");
                cb_turmas.SelectedValue = dt.Rows[0].Field<Int64>("N_IDTURMA");
                turmaAtual = cb_turmas.Text;
                pictureBox1.ImageLocation = dt.Rows[0].Field<string>("T_FOTO");
            }
        }

        private void btn_imprimir_Click(object sender, EventArgs e)
        {
            //pdf
        }
    }
}
