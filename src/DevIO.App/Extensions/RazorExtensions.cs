using Microsoft.AspNetCore.Mvc.Razor;

namespace DevIO.App.Extensions
{
    public static class RazorExtensions
    {
        public static string FormataDocumento(this RazorPage page, int tipoPessoa, string documento)
        {
            return tipoPessoa == 1 ? Convert.ToUInt64(documento).ToString(@"000\.000\.000\-00") : Convert.ToUInt64(documento).ToString(@"00\.000\.000\/0000\-00");
        }

        public static string MarcarOpcao(this RazorPage page, int tipoPessoa, int valor)
        {
            return tipoPessoa == valor ? "checked" : "";
        }

    }
}