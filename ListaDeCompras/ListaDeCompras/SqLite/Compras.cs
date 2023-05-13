using SQLite;
using System;

namespace ListaDeCompras.SqLite
{
    public class Compras
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; } 
        [Ignore]
        public decimal Total => Quantidade * ValorUnitario;
        public string Mes { get; set; }
        public int Ano { get; set; }
        public DateTime DataHora { get; set; }
        public string IdCompra { get; set; }
        public string Descricao { get; set; }
        public string TotalFormatted => Total.ToString("N2");
    }
}
