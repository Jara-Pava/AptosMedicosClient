using System.Collections.Generic;

namespace DataAccessDesarrollos
{
    public class ErrorResponseAptosMedicos
    {
        //public int StatusCode { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<string> Errores { get; set; } = new List<string>();
    }
}