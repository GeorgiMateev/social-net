using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNet.ViewModels.Users
{
    public class UserViewModel
    {
        public UserViewModel(string userName, string description)
        {
            Username = userName;
            Description = description;
        }

        public string Username { get; set; }

        public string Description { get; set; }
    }
}
