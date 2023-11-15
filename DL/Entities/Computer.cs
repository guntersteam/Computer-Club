using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace DL.Entities; 
public class Computer
{
    public int ComputerId { get; set; }
    public string ModelName { get; set; }
    public int PriceForHour { get; set; }
    public bool IsReserved { get; set; }
    
}
