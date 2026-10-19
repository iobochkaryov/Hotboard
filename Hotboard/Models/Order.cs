using System.ComponentModel.DataAnnotations;

namespace Hotboard.Models
{
    public enum OrderStatus
    {
        NotReady,
        Waiting,
        Ready,
        Printed,
        Posted,
        CalledIn,
        Complete
    }

    public class Order
    {
        public int Id { get; set; }

        [Display(Name = "SO#")]
        public string? SO { get; set; }

        [Display(Name = "Del#")]
        public string? Del { get; set; }

        [Display(Name = "PO#")]
        public string? PO { get; set; }

        [Display(Name = "Rec#")]
        public string? Rec { get; set; }

        [Display(Name = "MO#")]
        public string? MO { get; set; }

        [Display(Name = "RO#")]
        public string? RO { get; set; }

        // Links
        [Display(Name = "Link for Order")]
        public string? LinkForOrder { get; set; }

        [Display(Name = "Link for Incoming PO")]
        public string? LinkForIncomingPO { get; set; }

        [Display(Name = "Link for MO")]
        public string? LinkForMO { get; set; }

        [Display(Name = "Link for RO")]
        public string? LinkForRO { get; set; }

        public string? Customer { get; set; }
        public string? Worker { get; set; }
        public string? CSR { get; set; }
        public string? Courier { get; set; }
        public string? Comment { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.NotReady;
    }
}
