using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialNet.Models;

namespace SocialNet.ViewModels.Users
{
    public class UserViewModel
    {
        public UserViewModel(string userName, string description)
        {
            Username = userName;
            Description = description;
        }

        public UserViewModel(string id, string userName, string description) 
            : this(userName, description)
        {
            this.Id = id;
        }

        public UserViewModel(ApplicationUser publisher)
            : this(publisher.Id, publisher.UserName, publisher.Description)
        {
        }

        public string Id { get; set; } 

        public string Username { get; set; }

        public string Description { get; set; }
    }
}
