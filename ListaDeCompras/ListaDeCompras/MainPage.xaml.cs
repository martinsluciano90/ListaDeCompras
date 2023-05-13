using ListaDeCompras.SqLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        protected override async void OnAppearing()
        {
            ListCompras = await bancoDados.ObterProdutosAsyncDate(DateTime.Now.Date.ToString("MMMM"), DateTime.Now.Year);

            var resultado = ListCompras
            .GroupBy(c => c.IdCompra)
            .Select(g => new
            {
                IdCompra = g.Key,
                g.FirstOrDefault().Descricao,
                TotalQuantidade = g.Sum(c => c.Quantidade),
                TotalValor = g.Sum(c => c.Total).ToString("C")
            })
            .ToList();

            ListViewUltimasCompras.ItemsSource = resultado;
            base.OnAppearing();
        }

        private async void GoToList_Clicked(object sender, EventArgs e)
        {
            if (txtDescricao.Text != "")
            {
                await Navigation.PushAsync(new Index(txtDescricao.Text.Trim(), GerarNumeroAleatorio.GeraSenhaAleatoria()));
                txtDescricao.Text = "";
            }
            else
            {
                await DisplayAlert("Descrição", "Defina uma descrição para a nova compra!", "Ok");
            }
        }

        private async void ListViewUltimasCompras_ItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                var selectedItem = args.SelectedItem;
                string idCompra = (string)selectedItem.GetType().GetProperty("IdCompra").GetValue(selectedItem, null);
                string descricao = (string)selectedItem.GetType().GetProperty("Descricao").GetValue(selectedItem, null);

                await Navigation.PushAsync(new Index(descricao, idCompra));
            }
        }
    }
}
