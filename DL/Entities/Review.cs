using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DL.Entities;

public class Review
{
    public int ReviewId { get; set; }
    public string ReviewText { get; set; }
    public int Rating { get; set; }

    [ForeignKey("AppUser")]
    public string UserId { get; set; }
    [JsonIgnore]
    public virtual AppUser User { get; set; }

    [ForeignKey("Computer")]
    public int ComputerId { get; set; }
    [JsonIgnore]
    public virtual Computer Computer { get; set; }
}
