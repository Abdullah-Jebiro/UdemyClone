using Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string NameCategory { get; set; } = null!;
    }
}
