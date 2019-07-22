using System;
using GiaoDichInterface.demo;
using GiaoDichInterface.entity;
using GiaoDichInterface.model;

namespace GiaoDichInterface
{
    public class SHB : Program.GiaoDich
    {
        private static SHBAccountModel shbAccountModel;

        public SHB()
        {
            shbAccountModel = new SHBAccountModel();
        }

        public  void Login()
        {
            Program.currentLoggedInAccount = null;
            Console.Clear();
            Console.WriteLine("Tiến hành đăng nhập hệ thống SHB.");
            // Yêu cầu nhập username, password.
            Console.WriteLine("Vui lòng nhập usename: ");
            var username = Console.ReadLine();
            Console.WriteLine("Vui lòng nhập mật khẩu: ");
            var password = Console.ReadLine();
            // gọi đến model kiểm, nếu model trả về null thì báo đăng nhập sai.
            var shbAccount = SHBAccountModel.FindByUsernameandPassword(username, password);
            if (shbAccount == null)
            {
                Console.WriteLine("Sai thông tin tài khoản, vui lòng đăng nhập lại.");
                Console.WriteLine("Ấn phím bất kỳ để tiếp tục.");
                Console.ReadLine();
                return;
            }

            // trong trường hợp trả về khác null.
            // set giá trị vào biến currentLoggedInAccount.
            Program.currentLoggedInAccount = shbAccount;
            
        }
        
        public void RutTien()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Tiến hành rút tiền tại hệ thống SHB.");
                Console.WriteLine("Vui lòng nhập số tiền cần rút.");
                var amount = double.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }

                var transaction = new SHBTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    Type = 1,
                    Message = "Tiến hành rút tiền tại ATM với số tiền: " + amount,
                    Amount = amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };
                bool result = shbAccountModel.UpdateBalance(Program.currentLoggedInAccount, transaction);
            }
            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }
        }

        public void GuiTien()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Tien hanh gui tien tai he thong SHB");
                Console.WriteLine("Vui Long nhap so tien can gui");
                var amount = Double.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("So luong khong hop le, vui long nhap lai");
                    return;
                }

                var transaction = new SHBTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    Type = 2,
                    Message = "Tien hanh gui tien tai ATM voi so tien" + amount,
                    Amount =  amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };
                bool result = shbAccountModel.UpdateBalance(Program.currentLoggedInAccount, transaction);
            }
            else
            {
                Console.WriteLine("Vui long dang nhap de su dung chuc nang nay");
            }
        }

        public void ChuyenTien()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Tien hanh chuyen khoan tai he thong SHB.");
                Console.WriteLine("Vui long nhap so tien can chuyen khoan.");
                var amount = double.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("So luong khong hop le, vui long thu lai.");
                    return;
                }

                var transaction = new SHBTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    Type = 3,
                    Message = "Tien hanh chuyen khoan tai ATM voi so tien: " + amount,
                    Amount = amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 3
                };
                bool result = shbAccountModel.UpdateBalance(Program.currentLoggedInAccount, transaction);
            }
            else
            {
                Console.WriteLine("Vui long dang nhap de su dung chuc nang nay.");
            }
        }
            

        
    }
    
        
}
