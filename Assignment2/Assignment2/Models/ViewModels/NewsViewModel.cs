namespace Assignment2.Models.ViewModels
{
    public class NewsViewModel
    {
        public NewsBoard NewsBoard { get; set; }
        public IEnumerable<NewsBoard> NewsBoards { get; set; }
        public List<News> News { get; set; }

    }
}
                    