using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Util
{
    class Validator
    {
        public static void Assert(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new Exception("Unexpected: " + errorMessage);
            }
        }
    }
}
