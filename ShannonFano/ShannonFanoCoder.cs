using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesCompressionProject.ShannonFano
{
    public class ShannonFanoCoder
    {
        public void BuildCodes(List<Symbol> symbols)
        {
            BuildCodesRecursive(symbols, 0, symbols.Count - 1);
        }

        private void BuildCodesRecursive(List<Symbol> symbols, int start, int end)
        {
            if (start >= end) return;

            int total = symbols.Skip(start).Take(end - start + 1).Sum(s => s.Frequency);
            int half = total / 2;
            int split = start;
            int sum = 0;

            for (int i = start; i <= end; i++)
            {
                sum += symbols[i].Frequency;
                if (sum >= half)
                {
                    split = i;
                    break;
                }
            }

            for (int i = start; i <= split; i++)
                symbols[i].Code += "0";
            for (int i = split + 1; i <= end; i++)
                symbols[i].Code += "1";

            BuildCodesRecursive(symbols, start, split);
            BuildCodesRecursive(symbols, split + 1, end);
        }
    }

}
