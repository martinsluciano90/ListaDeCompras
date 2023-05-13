using ListaDeCompras.SqLite;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace ListaDeCompras
{
    public partial class MainPage : ContentPage
    {        
        private BancoDadosSQLite bancoDados;
        List<Compras> ListCompras;
        public MainPage()
        {            
            InitializeComponent();

            string Local = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bancoDadosSQLite.db3");

            bancoDados = new BancoDadosSQLite(Local);
        }       
    }
}
