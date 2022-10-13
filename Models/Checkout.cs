using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventRegistration.Models
{
    public class Checkout
    {
        public int ApplicantId { get; set; }
        public long OrderId { get; set; }
        public string pType { get; set; }
        public string Price { get; set; }

    }
}
