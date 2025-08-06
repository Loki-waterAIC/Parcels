using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcels
{
    public class ParcelSummarizer : IParcelSummarizer
    {
        public string GenerateSummary(ParcelSummary summary)
        {
            var message = $"Found {summary.Count} parcels.";
            message += $"\nCombined area: {summary.Area:N2}";
            return message;
        }
    }
}
