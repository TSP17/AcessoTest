using EasyNetQ;
using System;

namespace AcessoTeste.ThiagoPereira.Web.Application.Models
{
    [Queue("TransferJob")]
    public class TransferInfo
    {
        public string Id { get; set; }
        public string AccountOrigin { get; set; }
        public string AccountDestination { get; set; }
        public string Value { get; set; }
        public string DateCreated { get; set; }
        public string DateUpdatedStatus { get; set; }
        public string Status { get; set; }

    }
}
