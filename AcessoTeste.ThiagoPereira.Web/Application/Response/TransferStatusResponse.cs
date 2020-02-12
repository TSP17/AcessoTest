using System.Collections.Generic;

namespace AcessoTeste.ThiagoPereira.Web.Application.Response
{
    public class TransferStatusResponse
    {
        private readonly IList<string> _errors = new List<string>();
        public void AddError(string message)
        {
            _errors.Add(message);
        }
        public IEnumerable<string> Erros => _errors;
        public string Status { get; set; }
    }
}
