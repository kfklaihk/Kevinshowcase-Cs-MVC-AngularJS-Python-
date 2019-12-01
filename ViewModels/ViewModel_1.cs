using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class ViewModel_1
    {
        [Key]
        public string CurrentUser { get; set; }
        public string datme { get; set; }
        public IEnumerable<StkHoldingModel> StkHoldingModels { get; set; }
        public IEnumerable<CashholdingModel> CashHoldingModels { get; set; }
        public IEnumerable<txModel> txModels { get; set; }
        public IEnumerable<Pending_txModel> Pending_txModels { get; set; }
        public decimal mkt_price { get; set; }
        public decimal xrate { get; set; }
        public string order_status { get; set; }

    }
}