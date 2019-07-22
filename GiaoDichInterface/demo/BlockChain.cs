using System;
using GiaoDichInterface.entity;
using GiaoDichInterface.model;

namespace GiaoDichInterface
{
    public class BlockChain : Program.GiaoDich
    {
        private BlockchainAddressModel _model = new BlockchainAddressModel();
        public void Login()
        {
            Program.currentLoggedInAccount = null;
            Console.Clear();
            Console.WriteLine("Tien hanh dang nhap he thong Blockchain.");
            Console.WriteLine("Vui lòng nhập address: ");
            var address = Console.ReadLine();
            Console.WriteLine("Vui long nhap private key: ");
            var privatekey = Console.ReadLine();
            var blockchainAccount = _model.FindByAddressAndPrivateKey(address, privatekey);
            
            if (blockchainAccount == null)
            {
                Console.WriteLine("Sai dia chi, vui long nhap lai.");
                Console.WriteLine("An phim bat ki de tiep tuc.");
                Console.Read();
                return;
            }

            Program.CurrentAddress = blockchainAccount;
        }


        public void RutTien()
        {
            if (Program.CurrentAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Tien hanh rut tien tai he thong Blockchain.");
                Console.WriteLine("Vui long nhap so tien can rut.");
                var amount = double.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("So luong khong hop le, vui long thu lai.");
                    return;
                }

                var transaction = new BlockchainTransaction{
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.CurrentAddress.Address,
                    ReceiverAddress = Program.CurrentAddress.Address,
                    Type = BlockchainTransaction.TransactionType.WITHDRAW,
                    Amount = amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdateAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };
                
                bool result = _model.UpdateBalance(Program.CurrentAddress, transaction);
            }
            else
            {
                Console.WriteLine("Vui long dang nhap lai de su dung chuc nang nay.");
            }
        }

        public void GuiTien()
        {
            if (Program.CurrentAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Tien hanh gui tien tai he thong Blockchain.");
                Console.WriteLine("Vui long nhap so tien can gui.");
                var amount = double.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("So luong khong hop le, vui long thu lai.");
                    return;
                }

                var transaction = new BlockchainTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.CurrentAddress.Address,
                    ReceiverAddress = Program.CurrentAddress.Address,
                    Type = BlockchainTransaction.TransactionType.DEPOSIT,
                    Amount = amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdateAtMLS = DateTime.Now.Ticks,
                    Status = 2
                };
               
                bool result = _model.UpdateBalance(Program.CurrentAddress, transaction);
            }
            else
            {
                Console.WriteLine("Vui long dang nhap lai de su dung chuc nang nay.");
            }
        }

        public void ChuyenTien()
        {
            if (Program.CurrentAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Tien hanh chuyen khoan tai he thong BLockchain.");
                Console.WriteLine("Vui long nhap so tien can chuyen khoan.");
                var amount = double.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("So luong khong hop le, vui long thu lai.");
                    return;
                }

                var transaction = new BlockchainTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.CurrentAddress.Address,
                    ReceiverAddress = Program.CurrentAddress.Address,
                    Type = BlockchainTransaction.TransactionType.Tranfer,
                    Amount = amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdateAtMLS = DateTime.Now.Ticks,
                    Status = 3
                };
                bool result = _model.Tranfer(Program.CurrentAddress, transaction);
            }
            else
            {
                Console.WriteLine("Vui long dang nhap lai de su dung chuc nang nay.");
            }
        }
    }
}