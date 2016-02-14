using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace SocialNet.ViewModels.Manage
{
    public class IndexViewModel
    {
        public string Description { get; set; }

        public bool HasPassword { get; set; }

        public bool BrowserRemembered { get; set; }
    }
}
