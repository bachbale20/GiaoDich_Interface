namespace GiaoDichInterface.entity
{
    public class BlockchainTransaction
    {
        public string TransactionId { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public double Amount { get; set; }
        public long CreatedAtMLS { get; set; }
        public long UpdateAtMLS { get; set; }
        public int Status { get; set; }
        public TransactionType Type { get; set; }
        
        public enum TransactionType
        {
            WITHDRAW = 1,
            DEPOSIT = 2,
            Tranfer = 3
        }

        public BlockchainTransaction(string senderAddress, string receiverAddress, double amount)
        {
            SenderAddress = senderAddress;
            ReceiverAddress = receiverAddress;
            Amount = amount;
        }

        public BlockchainTransaction()
        {
        }
    }
}