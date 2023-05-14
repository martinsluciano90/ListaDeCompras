using Acr.UserDialogs;
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
    public partial class Index : ContentPage
    {
        private BancoDadosSQLite bancoDados;
        List<Compras> ListCompras;
        string NrBusca;
        public Index(string descricaoCompra, string StringAleatoria)
        {
            InitializeComponent();

            string Local = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bancoDadosSQLite.db3");
            bancoDados = new BancoDadosSQLite(Local);

            lblIdCompra.Text = StringAleatoria;
            lblDescricao.Text = descricaoCompra;
            NrBusca = StringAleatoria;
        }
        protected override async void OnAppearing()
        {
            ListCompras = await bancoDados.ObterProdutosAsync(NrBusca);
            ListViewCompras.ItemsSource = ListCompras.OrderBy(x => x.Nome);
            txtTotal.Text = ListCompras.Where(x => x.Mes.ToUpper() == DateTime.Now.Date.ToString("MMMM").ToUpper() && x.Ano == DateTime.Now.Year).Sum(x => x.Total).ToString("C");
            txtItens.Text = " / " + ListCompras.Where(x => x.Mes.ToUpper() == DateTime.Now.Date.ToString("MMMM").ToUpper() && x.Ano == DateTime.Now.Year).Sum(x => x.Quantidade).ToString() + " Itens";
            base.OnAppearing();

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

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            if (txtProduto.Text != "" && txtQuatidade.Text != "" && txtValorUnitario.Text != "")
            {
                var SaveProduto = new Compras
                {
                    Descricao = lblDescricao.Text,
                    IdCompra = lblIdCompra.Text,
                    DataHora = DateTime.Now,
                    Ano = DateTime.Now.Year,
                    Nome = txtProduto.Text,
                    Mes = DateTime.Now.Date.ToString("MMMM"),
                    Quantidade = Convert.ToDecimal(txtQuatidade.Text),
                    ValorUnitario = Convert.ToDecimal(txtValorUnitario.Text)
                };

                await bancoDados.AdicionarProdutosAsync(SaveProduto);
                ListCompras.Add(SaveProduto);
                if (ChqAZ.IsChecked)
                {
                    ListViewCompras.ItemsSource = ListCompras.OrderBy(x => x.Nome);
                }
                else
                {
                    ListViewCompras.ItemsSource = ListCompras.OrderByDescending(x => x.DataHora);
                }
                txtTotal.Text = ListCompras.Where(x => x.Mes.ToUpper() == DateTime.Now.Date.ToString("MMMM").ToUpper() && x.Ano == DateTime.Now.Year).Sum(x => x.Total).ToString("C");
                txtItens.Text = " / " + ListCompras.Where(x => x.Mes.ToUpper() == DateTime.Now.Date.ToString("MMMM").ToUpper() && x.Ano == DateTime.Now.Year).Sum(x => x.Quantidade).ToString() + " Itens";

                txtProduto.Text = "";
                txtQuatidade.Text = "1";
                txtValorUnitario.Text = "";

                UserDialogs.Instance.Toast("Produto Incluído!", TimeSpan.FromSeconds(1));
            }
        }

        private async void BtnAtualizar_Clicked(object sender, EventArgs e)
        {
            if (txtProduto.Text != "" && txtQuatidade.Text != "" && txtValorUnitario.Text != "")
            {
                var UpdateProduto = new Compras
                {
                    Descricao = lblDescricao.Text,
                    IdCompra = lblIdCompra.Text,
                    DataHora = DateTime.Now,
                    Ano = Convert.ToInt32(lblAno.Text),
                    Id = Convert.ToInt32(lblId.Text),
                    Nome = txtProduto.Text,
                    Mes = DateTime.Now.Date.ToString("MMMM"),
                    Quantidade = Convert.ToDecimal(txtQuatidade.Text),
                    ValorUnitario = Convert.ToDecimal(txtValorUnitario.Text)
                };

                await bancoDados.AtualizarProdutosAsync(UpdateProduto);
                ListCompras = await bancoDados.ObterProdutosAsync(NrBusca);
                if (ChqAZ.IsChecked)
                {
                    ListViewCompras.ItemsSource = ListCompras.OrderBy(x => x.Nome);
                }
                else
                {
                    ListViewCompras.ItemsSource = ListCompras.OrderByDescending(x => x.DataHora);
                }
                txtTotal.Text = ListCompras.Where(x => x.Mes.ToUpper() == DateTime.Now.Date.ToString("MMMM").ToUpper() && x.Ano == DateTime.Now.Year).Sum(x => x.Total).ToString("C");
                txtItens.Text = " / " + ListCompras.Where(x => x.Mes.ToUpper() == DateTime.Now.Date.ToString("MMMM").ToUpper() && x.Ano == DateTime.Now.Year).Sum(x => x.Quantidade).ToString() + " Itens";

                txtProduto.Text = "";
                txtQuatidade.Text = "1";
                txtValorUnitario.Text = "";

                UserDialogs.Instance.Toast("Produto Atualizado!", TimeSpan.FromSeconds(1));
            }
            else
            {
                await DisplayAlert("Produto", "Selecione ao menos um produto da lista para Atualizar!", "OK");
            }
        }

        private void ListViewCompras_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                if (e.Item is Compras ItemSelect)
                {
                    lblDescricao.Text = ItemSelect.Descricao;
                    lblIdCompra.Text = ItemSelect.IdCompra.ToString();
                    lblDataHora.Text = ItemSelect.DataHora.ToString();
                    lblAno.Text = ItemSelect.Ano.ToString();
                    lblId.Text = ItemSelect.Id.ToString();
                    txtProduto.Text = ItemSelect.Nome;
                    txtQuatidade.Text = ItemSelect.Quantidade.ToString();
                    txtValorUnitario.Text = ItemSelect.ValorUnitario.ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void BtnExcluir_Clicked(object sender, EventArgs e)
        {
            if (txtProduto.Text != "" && txtQuatidade.Text != "" && txtValorUnitario.Text != "")
            {
                var Dialogo = await DisplayAlert("Excluír", "Deseja Excluír? " + txtProduto.Text, "Sim", "Não");

                if (Dialogo == true)
                {
                    var DeleteProduto = new Compras
                    {
                        Descricao = lblDescricao.Text,
                        IdCompra = lblIdCompra.Text,
                        DataHora = DateTime.Now,
                        Ano = Convert.ToInt32(lblAno.Text),
                        Id = Convert.ToInt32(lblId.Text),
                        Nome = txtProduto.Text,
                        Mes = DateTime.Now.Date.ToString("MMMM"),
                        Quantidade = Convert.ToDecimal(txtQuatidade.Text),
                        ValorUnitario = Convert.ToDecimal(txtValorUnitario.Text)
                    };

                    await bancoDados.ExcluirProdutosAsync(DeleteProduto);
                    ListCompras = await bancoDados.ObterProdutosAsync(NrBusca);
                    if (ChqAZ.IsChecked)
                    {
                        ListViewCompras.ItemsSource = ListCompras.OrderBy(x => x.Nome);
                    }
                    else
                    {
                        ListViewCompras.ItemsSource = ListCompras.OrderByDescending(x => x.DataHora);
                    }
                    txtTotal.Text = ListCompras.Where(x => x.Mes.ToUpper() == DateTime.Now.Date.ToString("MMMM").ToUpper() && x.Ano == DateTime.Now.Year).Sum(x => x.Total).ToString("C");
                    txtItens.Text = " / " + ListCompras.Where(x => x.Mes.ToUpper() == DateTime.Now.Date.ToString("MMMM").ToUpper() && x.Ano == DateTime.Now.Year).Sum(x => x.Quantidade).ToString() + " Itens";

                    txtProduto.Text = "";
                    txtQuatidade.Text = "1";
                    txtValorUnitario.Text = "";

                    UserDialogs.Instance.Toast("Produto Excluído!", TimeSpan.FromSeconds(1));
                }
                else
                {
                    UserDialogs.Instance.Toast("Cancelado!", TimeSpan.FromSeconds(1));
                }
            }
            else
            {
                await DisplayAlert("Produto", "Selecione ao menos um produto da lista para excluír!", "OK");
            }
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

        private void txtProduto_SuggestionChosen(object sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            if (ChqPesquisa.IsChecked)
            {
                var itemselecionado = e.SelectedItem;
                ListViewCompras.ItemsSource = ListCompras.Where(x => x.Nome.ToUpper() == itemselecionado.ToString().ToUpper());
                txtTotal.Text = ListCompras.Where(x => x.Nome.ToUpper() == itemselecionado.ToString().ToUpper()).Sum(x => x.Total).ToString("C");
                txtItens.Text = " / " + ListCompras.Where(x => x.Nome.ToUpper() == itemselecionado.ToString().ToUpper()).Sum(x => x.Quantidade).ToString("") + " Itens";
            }
        }

        private void ChqPesquisa_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!ChqPesquisa.IsChecked)
            {
                if (ChqAZ.IsChecked)
                {
                    ListViewCompras.ItemsSource = ListCompras.OrderBy(x => x.Nome);
                    txtTotal.Text = ListCompras.OrderBy(x => x.Nome).Sum(x => x.Total).ToString("C");
                    txtItens.Text = " / " + ListCompras.OrderBy(x => x.Nome).Sum(x => x.Quantidade).ToString("") + " Itens";
                }
                else
                {
                    ListViewCompras.ItemsSource = ListCompras.OrderByDescending(x => x.Id);
                    txtTotal.Text = ListCompras.OrderBy(x => x.Nome).Sum(x => x.Total).ToString("C");
                    txtItens.Text = " / " + ListCompras.OrderBy(x => x.Nome).Sum(x => x.Quantidade).ToString("") + " Itens";
                }
            }
        }
    }
}