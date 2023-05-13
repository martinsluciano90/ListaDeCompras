using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ListaDeCompras.SqLite
{
    public class BancoDadosSQLite
    {
        private SQLiteAsyncConnection conexao;

        public BancoDadosSQLite(string caminho)
        {
            conexao = new SQLiteAsyncConnection(caminho);            
            conexao.CreateTableAsync<Compras>().Wait();
        }
        public async Task<List<Compras>> ObterProdutosAsync(string Mes, int Ano)
        {
            return await conexao.Table<Compras>().Where(x => x.Mes.ToUpper() == Mes.ToUpper() & x.Ano == Ano).ToListAsync();
        }
        public async Task<List<Compras>> ObterProdutos()
        {
            return await conexao.Table<Compras>().ToListAsync();
        }

        public async Task<int> AdicionarProdutosAsync(Compras compras)
        {
            return await conexao.InsertAsync(compras);
        }

        public async Task<int> AtualizarProdutosAsync(Compras compras)
        {
            return await conexao.UpdateAsync(compras);
        }

        public async Task<int> ExcluirProdutosAsync(Compras compras)
        {
            return await conexao.DeleteAsync(compras);
        }
    }
}
