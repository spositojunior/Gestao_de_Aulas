namespace SQLite
{
    public partial class F_SelecionarTurma : Form
    {
        F_NovoAluno formNovoAluno;
        public F_SelecionarTurma(F_NovoAluno f)
        {
            InitializeComponent();
            formNovoAluno = f;
        }

        private void F_SelecionarTurma_Load(object sender, EventArgs e)
        {
            string queryTurmas = string.Format(@"
                SELECT
                    tbt.N_IDTURMA as 'ID',
                    tbt.T_DSCTURMA as 'Turma',
                    tbh.T_DSCHORARIO as 'Horário',
                    tbp.T_NOMEPROFESSOR as 'Professor',
                    tbt.N_MAXALUNOS as 'Máx. Alunos',
                    (   SELECT
                            count(N_IDALUNO)
                        FROM
                            tb_alunos as tba
                        WHERE
                            tba.N_IDTURMA = tbt.N_IDTURMA and T_STATUS='A'
                    )as 'Qtde. Alunos'
                FROM
                    tb_turmas as tbt
                    
                INNER JOIN
                    tb_professores as tbp on tbp.N_IDPROFESSOR = tbt.N_IDPROFESSOR
                INNER JOIN
                    tb_horarios as tbh on tbh.N_IDHORARIO = tbt.N_IDHORARIO
            ");
            dgv_turmas.DataSource = Banco.DQL(queryTurmas);
            dgv_turmas.Columns[0].Width = 40;
            dgv_turmas.Columns[1].Width = 200;
            dgv_turmas.Columns[2].Width = 100;
            dgv_turmas.Columns[3].Width = 200;
            dgv_turmas.Columns[4].Width = 60;
            dgv_turmas.Columns[5].Width = 60;


        }

        private void Selecionar(object sender,EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            int maxAlunos = 0;
            int qtdeAluno = 0;
            maxAlunos = Int32.Parse(dgv.SelectedRows[0].Cells[4].Value.ToString());
            qtdeAluno = Int32.Parse(dgv.SelectedRows[0].Cells[5].Value.ToString());
            if(qtdeAluno >= maxAlunos)
            {
                MessageBox.Show("Não há vagas nesta turma");
                return;
            }
            else
            {
                formNovoAluno.tb_turma.Text = dgv.Rows[dgv.SelectedRows[0].Index].Cells[1].Value.ToString();
                formNovoAluno.tb_turma.Tag = dgv.Rows[dgv.SelectedRows[0].Index].Cells[0].Value.ToString();
                Close();
            }
        }
    }
}
