﻿using System.ComponentModel.DataAnnotations.Schema;

namespace DL.Entities;

public class Order
{
    public int OrderId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndTime { get; set; }
    public bool State { get; set; }

    [ForeignKey("AppUser")]
    public string UserId { get; set; }
    public AppUser User { get; set; }

    [ForeignKey("Computer")]
    public int ComputerId { get; set; }
    public Computer Computer { get; set; }

}

