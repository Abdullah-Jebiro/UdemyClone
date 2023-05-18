using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Core
{
    public class SoftDeletable
    {
     public bool IsDeleted{ get; set; } = false;
    }
}
