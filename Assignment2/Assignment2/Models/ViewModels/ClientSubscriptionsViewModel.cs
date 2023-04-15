namespace Assignment2.Models.ViewModels
{
    public class ClientSubscriptionsViewModel
    {
        public Client Client { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public List<NewsBoardSubscriptionsViewModel> Subscriptions { get; set; }

    }
}
