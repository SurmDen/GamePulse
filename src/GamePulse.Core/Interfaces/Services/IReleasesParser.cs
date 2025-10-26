using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Interfaces
{
    public interface IReleasesParser
    {
        public Task<List<long>> GetReleaseGamesIdsAsync(int month);
    }
}
