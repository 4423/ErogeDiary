using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models.ErogameScape
{
    public class GameInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ImageUri { get; set; }
    }
}
