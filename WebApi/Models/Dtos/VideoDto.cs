using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos
{
    public class VideoDto
    {
        public int VideoId { get; set; }
        public string VideoTitle { get; set; } = null!;
        public string VideoUrl { get; set; } = null!;
    }
}
