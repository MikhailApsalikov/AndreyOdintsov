using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Odintsov.Accounts.Web.Models
{
    public class AccountsFilter
    {
        public string Region { get; set; }
        public string MicroRegion { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
    }
}