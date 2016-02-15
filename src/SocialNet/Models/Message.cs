namespace SocialNet.Models
{
    public class Message
    {
        public string Id { get; set; }

        public ApplicationUser Author { get; set; }

        public string Text { get; set; }
    }
}
