using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNet.Models
{
    public class Subscription
    {
        public Subscription()
        {

        }

        public Subscription(ApplicationUser subscriber, ApplicationUser publisher)
        {
            this.Subscriber = subscriber;
            this.SubscriberId = subscriber.Id;
            this.Publisher = publisher;
            this.PublisherId = publisher.Id;
        }

        public string SubscriberId { get; set; }

        public ApplicationUser Subscriber { get; set; }

        public string PublisherId { get; set; }

        public ApplicationUser Publisher { get; set; }
    }
}
