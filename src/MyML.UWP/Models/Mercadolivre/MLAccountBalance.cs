using System;
using System.Collections.Generic;
using System.Text;

namespace MyML.UWP.Models.Mercadolivre
{
    public class UnavailableBalanceByReason
    {
        public string reason { get; set; }
        public double? amount { get; set; }
    }

    public class AvailableBalanceByTransactionType
    {
        public double? amount { get; set; }
        public string transaction_type { get; set; }
    }

    public class MLAccountBalance
    {
        public long? user_id { get; set; }
        public double? total_amount { get; set; }
        public double? available_balance { get; set; }
        public double? unavailable_balance { get; set; }
        public List<UnavailableBalanceByReason> unavailable_balance_by_reason { get; set; }
        public List<AvailableBalanceByTransactionType> available_balance_by_transaction_type { get; set; }
        public string currency_id { get; set; }
    }
}
