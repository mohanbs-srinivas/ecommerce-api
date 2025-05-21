namespace ecommerce_api.Models
{
    public class Properties
    {

    }
    public class Account
    {
        public string accountId { get; set; }
        public string accountUri { get; set; }
        public string accountName { get; set; }
        public Properties properties { get; set; }
    }
    public class Accounts
    {
        public int count { get; set; }
        public List<Account> value { get; set; }

    }
}
