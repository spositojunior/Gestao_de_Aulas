using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLite
{
    public partial class F_Login : Form
    {
        Form1 form1;
        DataTable dt = new();
        public F_Login(Form1 f)
        {
            InitializeComponent();
            form1 = f;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = tb_username.Text;
            string senha = tb_senha.Text;
            if(username == "" || senha == "")
            {
                MessageBox.Show("Usuário e ou senha inválidos");
                tb_username.Focus();
                return;
            }
            string sql = "SELECT * FROM tb_usuarios WHERE T_USERNAME='"+username+"' AND T_SENHAUSUARIO='"+senha+"'";
            dt = Banco.DQL(sql);
            if(dt.Rows.Count == 1)
            {
                form1.lb_acess.Text = form1.lb_acess.Tag + dt.Rows[0].ItemArray[5].ToString();
                form1.lb_user.Text = form1.lb_user.Tag + dt.Rows[0].Field<string>("T_NOMEUSUARIO");
                form1.pictureBox1.Image = Gestão_de_Aulas.Properties.Resources.led_verde1;
                Globais.nivel= int.Parse(dt.Rows[0].Field<Int64>("N_NIVELUSUARIO").ToString());
                Globais.logado = true;
                Close();
            }
            else
            {
                MessageBox.Show("Usuário não encontrado");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
