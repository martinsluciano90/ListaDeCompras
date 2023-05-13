using System;
using System.Linq;

namespace ListaDeCompras
{
    public class GerarNumeroAleatorio
    {
        private const string CaracteresPermitidos = "1234567890";

        public static string GeraSenhaAleatoria()
        {

            Random random = new Random();
            string senhaAleatoria = new string(Enumerable.Repeat(CaracteresPermitidos, 4)
                                          .Select(s => s[random.Next(s.Length)]).ToArray());
            return senhaAleatoria;
        }

    }
}
