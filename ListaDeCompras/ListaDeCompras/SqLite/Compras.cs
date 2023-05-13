using CsvHelper;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

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
    public static class ComprasHelper
    {
        public static void ExportarCsv(string caminhoArquivo, List<Compras> listaDeCompras)
        {
            using (var writer = new StreamWriter(caminhoArquivo))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(listaDeCompras);
            }
        }

        public static List<Compras> ImportarCsv(string caminhoArquivo)
        {
            using (var reader = new StreamReader(caminhoArquivo))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<Compras>().ToList();
            }
        }
    }
}
