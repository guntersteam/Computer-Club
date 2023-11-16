using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Entities;

public class Review
{
    public int ReviewId { get; set; }
    public string ReviewText { get; set; }
    public int Rating { get; set; }
    public string UserId { get; set; }

    [ForeignKey("AppUser")]
    public AppUser User { get; set; }
    public int ComputerId { get; set; }
    [ForeignKey("Computer")]
    public Computer Computer { get; set; }
}
