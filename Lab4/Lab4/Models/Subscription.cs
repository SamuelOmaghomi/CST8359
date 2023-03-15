namespace Lab4.Models
{
    public class Subscription
    {
        public int ClientId { get; set; }
        public string NewsBoardId { get; set; }
        public Client Client { get; set; }
        public NewsBoard NewsBoard { get; set; }
    }
}
