using dotMorten.Xamarin.Forms;
using ListaDeCompras.SqLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ListaDeCompras
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageListView : ContentPage
    {
        private BancoDadosSQLite bancoDados;
        List<Compras> ListCompras;
        public PageListView()
        {
            InitializeComponent();
            string Local = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bancoDadosSQLite.db3");
            bancoDados = new BancoDadosSQLite(Local);
        }
        protected override async void OnAppearing()
        {
            ListCompras = await bancoDados.ObterProdutosAsync(DateTime.Now.Date.ToString("MMMM"), DateTime.Now.Year);
            ListViewCompras.ItemsSource = ListCompras.OrderBy(x => x.Nome);            
            base.OnAppearing();
        }

        private void ChqAZ_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                if (ChqAZ.IsChecked)
                {
                    ListViewCompras.ItemsSource = ListCompras.OrderBy(x => x.Nome);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ChqOrdemDeCompra_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                if (ChqOrdemDeCompra.IsChecked)
                {
                    ListViewCompras.ItemsSource = ListCompras.OrderByDescending(x => x.Id);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void txtProduto_TextChanged(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxTextChangedEventArgs e)
        {
            AutoSuggestBox box = (AutoSuggestBox)sender;

            if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (string.IsNullOrWhiteSpace(box.Text))
                    box.ItemsSource = null;
                else
                {
                    var suggestions = await bancoDados.ObterProdutos();
                    box.ItemsSource = suggestions.Where(x => RemoverAcentos.Remover(x.Nome.ToUpper()).Contains(RemoverAcentos.Remover(box.Text.ToUpper()))).Select(x => x.Nome).Distinct().ToList();
                }
            }
        }

        private void txtProduto_SuggestionChosen(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            var itemselecionado = e.SelectedItem;
            ListViewCompras.ItemsSource = ListCompras.Where(x => x.Nome.ToUpper() == itemselecionado.ToString().ToUpper());
        }
    }
}