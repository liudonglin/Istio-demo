using System;

namespace appstore_appservice
{
    public class OrderInfo
    {
        public int OrderID { get; set; }

        public int AppID { get; set; }

        public decimal PaymentAmount { get; set; }

        public string CustomerAccount { get; set; }

        public int Score { get; set; }

        public string Evaluate { get; set; }

        public DateTime OrderDate { get; set; }
    }
}