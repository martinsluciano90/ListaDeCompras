using Acr.UserDialogs;
using dotMorten.Xamarin.Forms;
using ListaDeCompras.SqLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ListaDeCompras
{
    public partial class MainPage : ContentPage
    {        
        private BancoDadosSQLite bancoDados;
        List<Compras> ListCompras;
        string descricao, idCompra;
        public MainPage()
        {            
            InitializeComponent();

            string Local = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bancoDadosSQLite.db3");
            bancoDados = new BancoDadosSQLite(Local);
        }
        protected override async void OnAppearing()
        {
            await Sheet.CloseSheet();
            await Pesquisa(DateTime.Now.Date.ToString("MMMM"), DateTime.Now.Year);
            base.OnAppearing();
        }

        private async void GoToList_Clicked(object sender, EventArgs e)
        {
            string NumeroAleatorio = GerarNumeroAleatorio.GeraSenhaAleatoria();
            ListCompras = await bancoDados.ObterProdutosAsync(NumeroAleatorio);

            if (ListCompras.Count > 0)
            {
                await DisplayAlert("Erro", "Já esxiste uma compra com esse ID, tente novamente!", "Ok");
                return;
            }

            if (txtDescricao.Text != null)
            {
                await Navigation.PushAsync(new Index(txtDescricao.Text.Trim(), NumeroAleatorio));
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
                descricao = (string)selectedItem.GetType().GetProperty("Descricao").GetValue(selectedItem, null);
                idCompra = (string)selectedItem.GetType().GetProperty("IdCompra").GetValue(selectedItem, null);

                await Sheet.OpenSheet();
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

        private async void ToolBackUp_Clicked(object sender, EventArgs e)
        {
            try
            {
                string LocalPath = "/storage/emulated/0/Documents/BackUp.csv";
                ListCompras = await bancoDados.ObterProdutos();
                ComprasHelper.ExportarCsv(LocalPath, ListCompras);
                UserDialogs.Instance.Toast("Aquivo de backUp Salvo!", TimeSpan.FromSeconds(5));
            }
            catch (Exception)
            {
                UserDialogs.Instance.Toast("Ocorreu um erro ao fazer o BackUp!", TimeSpan.FromSeconds(5));
            }
        }

        private async void ToolImportar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var BackUpImports = ComprasHelper.ImportarCsv("/storage/emulated/0/Documents/BackUp.csv");

                foreach (var item in BackUpImports)
                {
                    await bancoDados.AdicionarProdutosAsync(item);
                }

                await Pesquisa(DateTime.Now.Date.ToString("MMMM"), DateTime.Now.Year);

                UserDialogs.Instance.Toast("Aquivo de backUp importado com sucesso!", TimeSpan.FromSeconds(5));
            }
            catch (Exception)
            {
                UserDialogs.Instance.Toast("Ocorreu um erro ao importar o BackUp!", TimeSpan.FromSeconds(5));
            }
        }

        private async void btnIrParaLista_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Index(descricao, idCompra));
        }

        private async void btnCompartilhar_Clicked(object sender, EventArgs e)
        {
            ListCompras = await bancoDados.ObterProdutosAsync(idCompra);

            string Dados = "";

            foreach (var item in ListCompras.OrderBy(x => x.Nome))
            {
                Dados += $"{item.Nome}\n";
            }

            await Sheet.CloseSheet();
            await Browser.OpenAsync("https://api.whatsapp.com/send?phone=55" + txtTel1.Text + "&text=" + Dados);
        }

        private async void ToolExcluirCompra_Clicked(object sender, EventArgs e)
        {
            var Dialogo = await DisplayAlert("Excluír", "Deseja Excluír? " + descricao, "Sim", "Não");

            ListCompras = await bancoDados.ObterProdutosAsync(idCompra);

            if (Dialogo == true)
            {
                foreach (var item in ListCompras)
                {
                    await bancoDados.ExcluirProdutosAsync(item);
                }

                await Sheet.CloseSheet();
                await Pesquisa(DateTime.Now.Date.ToString("MMMM"), DateTime.Now.Year);
            }
            else
            {
                await Sheet.CloseSheet();
                UserDialogs.Instance.Toast("Cancelado!", TimeSpan.FromSeconds(1));
            }
        }
    }
}
