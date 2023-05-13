using dotMorten.Xamarin.Forms;
using ListaDeCompras.SqLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            await Pesquisa(DateTime.Now.Date.ToString("MMMM"), DateTime.Now.Year);
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

        private async void txtMes_SuggestionChosen(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            try
            {
                var itemselecionado = e.SelectedItem;
                await Pesquisa(itemselecionado.ToString(), Convert.ToInt32(txtAno.Text));
            }
            catch (Exception)
            {
            }
        }

        private async void txtAno_SuggestionChosen(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            try
            {
                var itemselecionado = e.SelectedItem;
                await Pesquisa(txtMes.Text, Convert.ToInt32(itemselecionado.ToString()));
            }
            catch (Exception)
            {
            }
        }
        async Task Pesquisa(string Mes, int Ano)
        {
            ListCompras = await bancoDados.ObterProdutosAsyncDate(Mes, Ano);

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
        }

        private async void txtMes_TextChanged(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxTextChangedEventArgs e)
        {
            AutoSuggestBox box = (AutoSuggestBox)sender;

            if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (string.IsNullOrWhiteSpace(box.Text))
                    box.ItemsSource = null;
                else
                {
                    var suggestions = await bancoDados.ObterProdutos();
                    box.ItemsSource = suggestions.Where(x => RemoverAcentos.Remover(x.Mes.ToUpper()).Contains(RemoverAcentos.Remover(box.Text.ToUpper()))).Select(x => x.Mes).Distinct().ToList();
                }
            }
        }

        private async void txtAno_TextChanged(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxTextChangedEventArgs e)
        {
            AutoSuggestBox box = (AutoSuggestBox)sender;

            if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (string.IsNullOrWhiteSpace(box.Text))
                    box.ItemsSource = null;
                else
                {
                    var suggestions = await bancoDados.ObterProdutos();
                    box.ItemsSource = suggestions.Where(x => x.Ano.ToString().Contains(box.Text)).Select(x => x.Ano).Distinct().ToList();
                }
            }
        }
    }
}
