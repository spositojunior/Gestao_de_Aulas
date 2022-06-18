using System.Data;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Diagnostics;


namespace SQLite
{
    public partial class F_GestaoTurmas : Form
    {
        string idSelecionado;
        int modo = 0; // Modo: 0 = padrão / 1 = edição / 2 = inserção
        string vqueryDGV;
        public F_GestaoTurmas()
        {
            InitializeComponent();
        }

        private void F_GestaoTurmas_Load(object sender, EventArgs e)
        {
            vqueryDGV = @"
                SELECT
                    tbt.N_IDTURMA as 'ID',
                    tbt.T_DSCTURMA as 'Turma',
                    tbh.T_DSCHORARIO as 'Horário'
                FROM
                    tb_turmas as tbt
                INNER JOIN
                    tb_horarios as tbh on tbh.N_IDHORARIO = tbt.N_IDHORARIO
            ";
            dgv_turmas.DataSource = Banco.DQL(vqueryDGV);
            dgv_turmas.Columns[0].Width = 40;
            dgv_turmas.Columns[1].Width = 180;
            dgv_turmas.Columns[2].Width = dgv_turmas.Width - 223;

            //popular ComboBox Professores
            string vqueryProfessores = @"
                SELECT
                    N_IDPROFESSOR,
                    T_NOMEPROFESSOR
                FROM
                    tb_professores
                ORDER BY
                    N_IDPROFESSOR
            ";
            cb_professor.Items.Clear();
            cb_professor.DataSource = Banco.DQL(vqueryProfessores);
            cb_professor.DisplayMember = "T_NOMEPROFESSOR";
            cb_professor.ValueMember = "N_IDPROFESSOR";

            //Popular ComboBox Status
            Dictionary<string, string> st = new Dictionary<string, string>();
            st.Add("A", "Ativa");
            st.Add("P", "Paralisada");
            st.Add("C", "Cancelada");
            cb_status.Items.Clear();
            cb_status.DataSource = new BindingSource(st,null);
            cb_status.DisplayMember = "Value";
            cb_status.ValueMember = "Key";

            //Popular ComboBox Horarios
            string vqueryHorarios = @"
                SELECT
                    *
                FROM
                    tb_horarios
                ORDER BY
                    T_DSCHORARIO
            ";
            cb_horario.Items.Clear();
            cb_horario.DataSource = Banco.DQL(vqueryHorarios);
            cb_horario.DisplayMember = "T_DSCHORARIO";
            cb_horario.ValueMember = "N_IDHORARIO";
        }

        private void dgv_turmas_SelectionChanged(object sender, EventArgs e)
        {
            btn_excluir.Enabled = true;
            btn_salvar.Text = "Salvar Edições";

            tb_turma.BackColor = SystemColors.Window;
            cb_professor.BackColor = SystemColors.Window;
            n_alunos.BackColor = SystemColors.Window;
            cb_status.BackColor = SystemColors.Window;
            cb_horario.BackColor = SystemColors.Window;
            //ultimo tb

            DataGridView dgv = (DataGridView)sender;
            int contLinhas = dgv.SelectedRows.Count;
            if (contLinhas > 0)
            {
                modo = 0;
                idSelecionado = dgv_turmas.Rows[dgv_turmas.SelectedRows[0].Index].Cells[0].Value.ToString();
                string vqueryCampos = @"
                    SELECT
                        T_DSCTURMA,
                        N_IDPROFESSOR,
                        N_IDHORARIO,
                        N_MAXALUNOS,
                        T_STATUS
                    FROM
                        tb_turmas
                    WHERE
                        N_IDTURMA="+idSelecionado;
                DataTable dt = Banco.DQL(vqueryCampos);
                tb_turma.Text = dt.Rows[0].Field<string>("T_DSCTURMA");
                cb_professor.SelectedValue = dt.Rows[0].Field<Int64>("N_IDPROFESSOR").ToString();
                n_alunos.Value= dt.Rows[0].Field<Int64>("N_IDPROFESSOR");
                cb_status.SelectedValue = dt.Rows[0].Field<string>("T_STATUS");
                cb_horario.SelectedValue = dt.Rows[0].Field<Int64>("N_IDHORARIO");
                tb_vagas.Text = CalcVagas();

            }
        }
        private string CalcVagas()
        {
            string queryVagas = string.Format(@"
                    SELECT
                        count(N_IDALUNO) as 'contVagas'
                    FROM
                        tb_alunos
                    WHERE
                        T_STATUS='A' and N_IDTURMA={0}", idSelecionado);
            DataTable dt = Banco.DQL(queryVagas);
            int vagas = Int32.Parse(Math.Round(n_alunos.Value, 0).ToString());
            vagas -= Int32.Parse(dt.Rows[0].Field<Int64>("contVagas").ToString());
            tb_vagas.Text = vagas.ToString();
            return vagas.ToString();
        }
        private void Modo_Changed(object sender, EventArgs e)
        {
            if(modo == 0)modo = 1;
            //if (sender == (NumericUpDown)sender) CalcVagas();
        }

        private void btn_novo_Click(object sender, EventArgs e)
        {
            dgv_turmas.ClearSelection();
            btn_excluir.Enabled = false;
            btn_salvar.Text = "Salvar Nova Turma";

            tb_turma.BackColor = SystemColors.Info;
            cb_professor.BackColor = SystemColors.Info;
            n_alunos.BackColor = SystemColors.Info;
            cb_status.BackColor = SystemColors.Info;
            cb_horario.BackColor = SystemColors.Info;
            //ultimo tb

            tb_turma.Clear();
            cb_professor.SelectedIndex = -1;
            n_alunos.Value = 0;
            cb_status.SelectedIndex = -1;
            cb_horario.SelectedIndex = -1;
            tb_turma.Focus();
            modo = 2;
        }

        private void btn_salvar_Click(object sender, EventArgs e)
        {
            if (modo != 0)
            {
                string queryTurma;
                string msg = "";
                if (modo == 1)
                {
                    msg = "Dados alterados";
                    queryTurma = String.Format(@"
                UPDATE
                    tb_turmas
                SET
                    T_DSCTURMA='{0}',
                    N_IDPROFESSOR={1},
                    N_IDHORARIO={2},
                    N_MAXALUNOS={3},
                    T_STATUS='{4}'
                WHERE
                    N_IDTURMA={5}", tb_turma.Text, cb_professor.SelectedValue, cb_horario.SelectedValue, Int32.Parse(Math.Round(n_alunos.Value, 0).ToString()), cb_status.SelectedValue, idSelecionado);
                }
                else
                {
                    msg = "Nova turma inserida";
                    queryTurma = String.Format(@"
                        INSERT INTO tb_turmas
                        (T_DSCTURMA,N_IDPROFESSOR,N_IDHORARIO,N_MAXALUNOS,T_STATUS)
                        VALUES('{0}',{1},{2},{3},'{4}')", tb_turma.Text, cb_professor.SelectedValue, cb_horario.SelectedValue, Int32.Parse(Math.Round(n_alunos.Value, 0).ToString()), cb_status.SelectedValue);
                }
                int linha = dgv_turmas.SelectedRows[0].Index;
                
                Banco.DML(queryTurma);
                if (modo == 1)
                {
                    dgv_turmas[1, linha].Value = tb_turma.Text;
                    dgv_turmas[2, linha].Value = cb_horario.Text;
                    tb_vagas.Text = CalcVagas();
                }
                else
                {
                    dgv_turmas.DataSource = Banco.DQL(vqueryDGV);
                }
                MessageBox.Show(msg);
            }
        }

        private void btn_excluir_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Confirma exclusão", "Excluir?", MessageBoxButtons.YesNo);
            if (res == DialogResult.No) return;
            string queryExcluirTurma = string.Format(@"
                DELETE
                FROM
                    tb_turmas
                WHERE
                    N_IDTURMA={0}",idSelecionado);
            Banco.DML(queryExcluirTurma);
            dgv_turmas.Rows.Remove(dgv_turmas.CurrentRow);
        }
        private async void btn_imprimir_Click(object sender, EventArgs e)
        {
            string nomeArquivo = Globais.caminho + "turma.pdf";
            Cursor = Cursors.AppStarting;
            PdfWriter wPdf = new(nomeArquivo, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            
            var pdfDocument = new PdfDocument(wPdf);

            var document = new Document(pdfDocument, PageSize.A4);
            document.SetMargins(50, 50, 50, 50);
            document.SetTextAlignment(TextAlignment.CENTER);

            //xx
            Paragraph p1 = new("Relatório de Turmas\n\n");
            p1.SetFontSize(20);
            p1.SetBold();

            Paragraph p2 = new("Curso de C#");
            p2.SetFontSize(11);

            float[] columnWidths = { 20, 70, 30 };
            Table tabel = new Table(UnitValue.CreatePercentArray(columnWidths)).UseAllAvailableWidth();
            tabel.AddCell("ID Turma");
            tabel.AddCell("Turma");
            tabel.AddCell("Horário");
            for(int x = 0; x < columnWidths.Length; x++)
            {
                tabel.GetCell(0,x).SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY);
            }

            DataTable dtTurmas = Banco.DQL(vqueryDGV);
            for (int i = 0; i < dtTurmas.Rows.Count; i++)
            {
                tabel.AddCell(dtTurmas.Rows[i].Field<Int64>("ID").ToString());
                tabel.AddCell(dtTurmas.Rows[i].Field<string>("Turma"));
                tabel.AddCell(dtTurmas.Rows[i].Field<string>("Horário"));
            }
            document.Add(p1);
            document.Add(tabel);
            document.Add(p2);

            document.Close();
            pdfDocument.Close();

            Cursor = Cursors.Default;
            if (DialogResult.Yes == MessageBox.Show("Relatório Salvo. Deseja abrir o Relatório?", "Relatório", MessageBoxButtons.YesNo))
            {
                Process pro = new();
                pro.StartInfo = new ProcessStartInfo(nomeArquivo)
                {
                    UseShellExecute = true
                };
                pro.Start();
            }
        }
        private void btn_fechar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
