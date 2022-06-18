namespace SQLite
{
    public partial class F_NovoAluno : Form
    {
        string origemCompleto;
        string foto;
        string pastaDestino;
        string destinoCompleto;
        public F_NovoAluno()
        {
            InitializeComponent();
        }

        private void F_NovoAluno_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> status = new Dictionary<string, string>();
            status.Add("A", "Ativo");
            status.Add("B", "Bloqueado");
            status.Add("C", "Cancelado");
            cb_status.DataSource = new BindingSource(status,null);
            cb_status.DisplayMember = "Value";
            cb_status.ValueMember = "Key";
        }

        private void btn_novo_Click(object sender, EventArgs e)
        {
            bool vai = true;
            AtivarDesativar(vai);
            cb_status.SelectedIndex = 0;
            tb_nome.Focus();
            tb_turma.Clear();
        }

        private void btn_cancelar_Click(object sender, EventArgs e)
        {
            bool vai = false;
            AtivarDesativar(vai);
            cb_status.SelectedIndex = 0;
            btn_novo.Focus();
            tb_nome.Clear();
            mtb_telefone.Clear();
            tb_turma.Clear();
        }

        private void btn_gravar_Click(object sender, EventArgs e)
        {
            if (destinoCompleto == "")
            {
                if (MessageBox.Show("Sem foto selecionada, deseja continuar?", "ERRO", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }
            if(destinoCompleto != "")
            {
                System.IO.File.Copy(origemCompleto, destinoCompleto, true);
                if (File.Exists(destinoCompleto)) pb_foto.ImageLocation = destinoCompleto;
                else
                {
                    if(MessageBox.Show("Erro ao localizar foto, deseja continuar?", "ERRO", MessageBoxButtons.YesNo) == DialogResult.No) return;
                }
            }
            string queryInsertAluno = string.Format(@"
                INSERT INTO tb_alunos
                (T_NOMEALUNO,T_TELEFONE,T_STATUS,N_IDTURMA,T_FOTO)
                VALUES('{0}','{1}','{2}',{3},'{4}')
            ",tb_nome.Text,mtb_telefone.Text,cb_status.SelectedValue,tb_turma.Tag.ToString(),destinoCompleto);
            Banco.DML(queryInsertAluno);
            MessageBox.Show("Novo Aluno inserido");

            bool vai = false;
            AtivarDesativar(vai);
            cb_status.SelectedIndex = 0;
            btn_novo.Focus();
            tb_nome.Clear();
            mtb_telefone.Clear();
            tb_turma.Clear();
            pb_foto.Image = null;
        }
        private void AtivarDesativar(bool b)
        {
            tb_nome.Enabled = b;
            mtb_telefone.Enabled = b;
            cb_status.Enabled = b;
            tb_plano.Enabled = b;
            tb_turma.Enabled = b;
            btn_dot1.Enabled = b;
            btn_dot2.Enabled = b;
            btn_gravar.Enabled = b;
            btn_cancelar.Enabled = b;
            btn_addFoto.Enabled = b;
            btn_novo.Enabled = (b==true)?false:true;
        }

        private void btn_fechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_dot1_Click(object sender, EventArgs e)
        {
            F_SelecionarTurma f_SelecionarTurma = new F_SelecionarTurma(this);
            f_SelecionarTurma.ShowDialog();
        }

        private void btn_dot2_Click(object sender, EventArgs e)
        {

        }

        private void btn_addFoto_Click(object sender, EventArgs e)
        {
            origemCompleto = "";
            foto= "";
            pastaDestino = Globais.caminhoFotos;
            destinoCompleto = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                origemCompleto = openFileDialog1.FileName;
                foto=openFileDialog1.SafeFileName;
                destinoCompleto = pastaDestino + foto;
            }
            if (File.Exists(destinoCompleto))
            {
                if (MessageBox.Show("Arquivo já existe, deseja substituir?", "Substituir", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }
            //System.IO.File.Copy(origemCompleto, destinoCompleto, true);
            //if (File.Exists(destinoCompleto)) pb_foto.ImageLocation = destinoCompleto;
            //else MessageBox.Show("Arquivo não copiado");
            pb_foto.ImageLocation = origemCompleto;
        }
    }
}
