using System;
using GiaoDichInterface.controller;
using GiaoDichInterface.entity;
using MySql.Data.MySqlClient;

namespace GiaoDichInterface.model
{
    public class SHBAccountModel
    {
        // Bình thường không làm theo cách này,
        // phải mã hoá mật khẩu, kiểm tra tài khoản theo username sau đó so sánh mật khẩu sau khi mã hoá.
        public bool UpdateBalance(SHBAccount currentLoggedInAccount, SHBTransaction transaction)
        {
            // 1. Kiểm tra số dư tài khoản hiện tại.
            // 2. Update số dư tài khoản hiện tại.
            // 3. Lưu thông tin giao dịch.
            // 4. Commit transaction.
            ConnectionHelper.GetConnection();
            var tran = ConnectionHelper.GetConnection().BeginTransaction(); // mở giao dịch.
            try
            {
                var cmd = new MySqlCommand("select * from SHBAccount where AccountNumber = @AccountNumber",
                    ConnectionHelper.GetConnection());
                cmd.Parameters.AddWithValue("@AccountNumber", currentLoggedInAccount.AccountNumber);
                SHBAccount shbAccount = null;
                var reader = cmd.ExecuteReader();
                double currentAccountBalance = 0;

                if (reader.Read())
                {
                    currentAccountBalance = reader.GetDouble("balance");
                }

                reader.Close();
                if (currentAccountBalance < 0)
                {
                    Console.WriteLine("Không đủ tiền trong tài khoản.");
                    return false;
                }

                if (transaction.Type == 1)
                {
                    if (currentAccountBalance < transaction.Amount)
                    {
                        Console.WriteLine("Khong du tien thuc hien giao dich");
                        return false;
                    }

                    currentAccountBalance -= transaction.Amount;
                }
                else if (transaction.Type == 2)
                {
                    currentAccountBalance += transaction.Amount;
                }

                var updateQuery =
                    "update `SHBAccount` set `balance` = @balance where accountNumber = @accountNumber";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@accountNumber", currentLoggedInAccount.AccountNumber);
                var updateResult = sqlCmd.ExecuteNonQuery();
                var historyTransactionQuery =
                    "insert into `SHBTransaction` (transactionId, type, senderAccountNumber, receiverAccountNumber, amount, message) " +
                    "values (@id, @type, @senderAccountNumber, @receiverAccountNumber, @amount, @message)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@id", transaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", transaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@type", transaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@message", transaction.Message);
                historyTransactionCmd.Parameters.AddWithValue("@senderAccountNumber",
                    transaction.SenderAccountNumber);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAccountNumber",
                    transaction.ReceiverAccountNumber);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();

                if (updateResult != 1 || historyResult != 1)
                {
                    throw new Exception("Không thể thêm giao dịch hoặc update tài khoản.");
                }

                tran.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                tran.Rollback(); // lưu giao dịch vào.                
                return false;
            }

            ConnectionHelper.CloseConnection();
            return true;
        }

        public static SHBAccount FindByUsernameandPassword(string username, string password)
        {
            // Tạo connection đến db, lấy ra trong bảng shb account những tài khoản có username, password trùng.            
            var cmd = new MySqlCommand("select * from SHBAccount where Username = @Username And Password = @Password",
                ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);
            // Tạo ra một đối tượng của lớp shbAccount.
            SHBAccount shbAccount = null;
            // Đóng Connection và trả về đối tượng này.          
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                shbAccount = new SHBAccount
                {
                    AccountNumber = reader.GetString("AccountNumber"),
                    Username = reader.GetString("username"),
                    Password = reader.GetString("password"),
                    Balance = reader.GetDouble("balance")
                };
            }

            reader.Close();
            ConnectionHelper.CloseConnection();
            // Trong trường hợp không tìm thấy tài khoản thì trả về null.
            return shbAccount;
        }

        public bool Tranfer(SHBAccount currentLoggedInAccount, SHBTransaction shbTransaction)
        {
            var conn = ConnectionHelper.GetConnection();

            var myTransaction = conn.BeginTransaction();
            try
            {
                var balanceSender = new MySqlCommand("select * from shbaccount where AccountNumber = @AccountNumber ",
                    conn);
                balanceSender.Parameters.AddWithValue("@AccountNumber", currentLoggedInAccount.AccountNumber);
                double currentAccountBalance = 0;
                var reader = balanceSender.ExecuteReader();
                if (reader.Read())
                {
                    currentAccountBalance = reader.GetDouble("Balance");
                }

                reader.Close();
                if (currentAccountBalance < shbTransaction.Amount)
                {
                    throw new Exception("Không đủ tiền trong tài khoản.");
                }

                currentAccountBalance -= shbTransaction.Amount;


                var updateQuery = ("update shbaccount set Balance = @balance where AccountNumber = @AccountNumber");
                var sqlCmd = new MySqlCommand(updateQuery, conn);
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@AccountNumber", currentLoggedInAccount.AccountNumber);
                var updateRs = sqlCmd.ExecuteNonQuery();

                var balanceReceiver = new MySqlCommand("select * from accounts where AccountNumber = @AccountNumber ",
                    conn);
                balanceReceiver.Parameters.AddWithValue("@AccountNumber", shbTransaction.ReceiverAccountNumber);
                double receiverBalance = 0;
                var readerReceiver = balanceReceiver.ExecuteReader();
                if (readerReceiver.Read())
                {
                    receiverBalance = readerReceiver.GetDouble("balance");
                }

                readerReceiver.Close();

                receiverBalance += shbTransaction.Amount;


                var updateQueryReceiver =
                    ("update shbaccount set Balance = @balance where AccountNumber = @AccountNumber");
                var sqlCmdReceiver = new MySqlCommand(updateQueryReceiver, conn);
                sqlCmdReceiver.Parameters.AddWithValue("@balance", receiverBalance);
                sqlCmdReceiver.Parameters.AddWithValue("@AccountNumber", shbTransaction.ReceiverAccountNumber);
                var updateResultReceiver = sqlCmdReceiver.ExecuteNonQuery();

                var historyTransactionQuery =
                    "insert into shbtransaction (TransactionId, Type, SenderAddress, ReceiverAddress, Amount, Message, CreatedAtMLS, UpdateAtMLS, Status) " +
                    "values (@id, @type, @senderAccountNumber, @receiverAccountNumber, @amount, @message, @createdAtMLS, @updatedAtMLS, @status)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, conn);
                historyTransactionCmd.Parameters.AddWithValue("@id", shbTransaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@type", shbTransaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@senderAccountNumber",
                    shbTransaction.SenderAccountNumber);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAccountNumber",
                    shbTransaction.ReceiverAccountNumber);
                historyTransactionCmd.Parameters.AddWithValue("@amount", shbTransaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@message", shbTransaction.Message);
                historyTransactionCmd.Parameters.AddWithValue("@createdAtMLS", shbTransaction.CreatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@updatedAtMLS", shbTransaction.UpdatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@status", shbTransaction.Status);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();

                if (updateRs != 1 || historyResult != 1 || updateResultReceiver != 1)
                {
                    throw new Exception("Không thể thêm giao dịch hoặc update tài khoản.");
                }

                myTransaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                myTransaction.Rollback();
                return false;
            }
            finally
            {
                conn.Close();
            }
        }


        public SHBAccount GetAccountByAccountNumber(string accountNumber)
        {
            var queryString = "select * from `shbaccount` where `AccountNumber` = @accountNumber ";
            var cmd = new MySqlCommand(queryString, ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@accountNumber", accountNumber);
            var reader = cmd.ExecuteReader();
            SHBAccount account = null;
            if (reader.Read())
            {
                account = new SHBAccount()
                {
                    AccountNumber = reader.GetString("AccountNumber"),
                    Username = reader.GetString("Username"),
                    Password = reader.GetString("Password"),
                    Balance = reader.GetDouble("Balance"),
                };
            }

            reader.Close();
            ConnectionHelper.CloseConnection();
            return account;
        }
    }
}