﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SocialNet.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Publishers = new List<Subscription>();
            this.Subscribers = new List<Subscription>();
        }

        public string Description { get; set; }

        public List<Subscription> Publishers { get; set; }

        public List<Subscription> Subscribers { get; set; }
    }
}
