using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDiary.Models.ErogameScape
{
    public interface IErogameScapeAccess
    {
        public Task<GameInfo> GetGameInfoFromGamePageUrl(string url);
    }
}
