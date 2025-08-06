using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcels
{
    public class ParcelSummary
    {
        public int Count { get; set; }
        public double Area { get; set; }

        public void Write(IParcelSummarizer summarizer, IMessageWriter writer)
        {
            writer.WriteMessage(summarizer.GenerateSummary(this));
        }
    };
}
