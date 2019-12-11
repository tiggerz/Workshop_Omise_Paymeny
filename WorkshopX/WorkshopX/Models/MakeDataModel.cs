using System;
using System.Collections.Generic;
using System.Text;

namespace WorkshopX.Models
{
    public class MakeDataModel
    {
        public string TokenId { get; set; }
        public int MemberId { get; set; }
        public int CarId { get; set; }
        public string PickUpDate { get; set; }
        public string ReturnDate { get; set; }
        public string InsurancePlanName { get; set; }
        public decimal InsurancePrice { get; set; }
        public decimal VatPercent { get; set; }
        public decimal RentalPrice { get; set; }
        public int MyProperty { get; set; }
        public decimal Vat { get; set; }
        public int BookingDuration { get; set; }
        public decimal TotalPrice { get; set; }


    }
}
