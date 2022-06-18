using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite
{
    internal class Globais
    {
        public static string versao = "1.0";
        public static bool logado = false;
        public static int nivel = 0;//1=básico - 1=gerente - 2=master
        //public static string caminho = System.Environment.CurrentDirectory; // antig
        public static string caminho = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
        public static string nomeBanco = "bd_sqlite.db";
        public static string caminhoBanco = caminho+@"\bd\";
        public static string caminhoFotos = caminho + @"\fotos\";
    }
}
