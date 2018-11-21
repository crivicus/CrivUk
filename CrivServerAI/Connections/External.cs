using CrivServer.AI.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.AI.Connections
{
    public class External
    {
        public ActionResult<IEnumerable<string>> GetFuzzyList(string similarTo)
        {
            using (Homebrew hb = new Homebrew())
            {
                return hb.RetrieveFromMemory(similarTo).ToList();
            }
        }
    }
}
