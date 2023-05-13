using System.Globalization;
using System.Text;

namespace ListaDeCompras
{
    public static class RemoverAcentos
    {
        public static string Remover(string texto)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = texto.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString();
        }
    }
}
