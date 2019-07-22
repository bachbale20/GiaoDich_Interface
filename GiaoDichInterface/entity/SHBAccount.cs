namespace GiaoDichInterface.entity
{
    public class SHBAccount
    {
        public string AccountNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }
        public long CreatedAtMLS { get; set; }
        public long UpdatedAtMLS { get; set; }

        public SHBAccount()
        {
        }
    }
}