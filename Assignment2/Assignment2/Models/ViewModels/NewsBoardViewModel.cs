namespace Assignment2.Models.ViewModels
{
    public class NewsBoardViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<NewsBoard> NewsBoards { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
