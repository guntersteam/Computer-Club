using DL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Entities;

public class Payment
{
    public int PaymentId { get; set; }
    public PaymentMethod PaymentType { get; set; }
    public int Amount { get; set; }
    public DateTime PaymentDate { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }

    public virtual Order Order { get; set; }
}
