using System;
using GiaoDichInterface.controller;
using GiaoDichInterface.entity;
using MySql.Data.MySqlClient;

namespace GiaoDichInterface.model
{
    public class BlockchainAddressModel
    {
        public BlockchainAddress FindByAddressAndPrivateKey(string address, string privatekey)
        {
            var cmd = new MySqlCommand(
                "select * from BlockchainAddress where Address = @Address And PrivateKey = @PrivateKey ",
                ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@Address", address);
            cmd.Parameters.AddWithValue("@PrivateKey", privatekey);
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            var blockchainAddress = new BlockchainAddress
            {
                Address = reader.GetString("Address"),
                PrivateKey = reader.GetString("PrivateKey"),
                Balance = reader.GetString("Balance")
            };
            reader.Close();

            Console.WriteLine("address" + blockchainAddress.Address);
            ConnectionHelper.GetConnection();

            return blockchainAddress;
        }


        public bool UpdateBalance(BlockchainAddress currentAddress, BlockchainTransaction blockchainTransaction)
        {
            ConnectionHelper.GetConnection();
            var tran = ConnectionHelper.GetConnection().BeginTransaction();
            try
            {
                var cmd = new MySqlCommand("select * from BlockchainAddress where Address = @Address",
                    ConnectionHelper.GetConnection());
                cmd.Parameters.AddWithValue("@Address", currentAddress.Address);
                var reader = cmd.ExecuteReader();
                double currentAddressBalance = 0;

                if (reader.Read())
                {
                    currentAddressBalance = reader.GetDouble("balance");
                }

                reader.Close();
                if (currentAddressBalance < 0)
                {
                    Console.WriteLine("Khong du tien");
                    return false;
                }

                if (blockchainTransaction.Type == BlockchainTransaction.TransactionType.WITHDRAW)
                {
                    if (currentAddressBalance < blockchainTransaction.Amount)
                    {
                        Console.WriteLine("Khong du tien de thuc hien giao dich");
                        return false;
                    }

                    currentAddressBalance -= blockchainTransaction.Amount;
                }
                else if (blockchainTransaction.Type == BlockchainTransaction.TransactionType.DEPOSIT)
                {
                    currentAddressBalance += blockchainTransaction.Amount;
                }

                var updateQuery =
                    "update `blockchainaddress` set `balance` = @balance where Address = @Address";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAddressBalance);
                sqlCmd.Parameters.AddWithValue("@address", currentAddress.Address);
                var updateResult = sqlCmd.ExecuteNonQuery();


                var historyTransactionQuery =
                    "insert into `blockchaintransaction` (TransactionId, Type, Amount, SenderAddress, ReceiverAddress, CreatedAtMLS, UpdateAtMLS)" +
                    "values (@id, @type, @amount, @SenderAddress, @ReceiverAddress, @CreatedAtMLS, @UpdateAtMLS)";

                var historyTransactionCmd = new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@id", blockchainTransaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", blockchainTransaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@type", blockchainTransaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@message", blockchainTransaction.Status);
                historyTransactionCmd.Parameters.AddWithValue("@SenderAddress",
                    blockchainTransaction.SenderAddress);
                historyTransactionCmd.Parameters.AddWithValue("@ReceiverAddress",
                    blockchainTransaction.ReceiverAddress);
                historyTransactionCmd.Parameters.AddWithValue("@CreatedAtMLS",
                    blockchainTransaction.CreatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@UpdateAtMLS",
                    blockchainTransaction.UpdateAtMLS);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();
                if (updateResult != 1 || historyResult != 1)
                {
                    throw new Exception("Khong the giao dich hoac them tai khoan.");
                }

                tran.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                tran.Rollback();
                return false;
            }

            ConnectionHelper.GetConnection();
            return true;
        }

        public bool Tranfer(BlockchainAddress currentLoggedInAddress, BlockchainTransaction transactionHb)
        {
            var conn = ConnectionHelper.GetConnection();

            var myTransaction = conn.BeginTransaction();
            try
            {
                var balanceSender = new MySqlCommand("select * from blockchainaddress where address = @address ",
                    conn, myTransaction);
                balanceSender.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);

                double currentAccountBalance = 0;
                var reader = balanceSender.ExecuteReader();
                if (reader.Read())
                {
                    currentAccountBalance = reader.GetDouble("balance");
                }

                reader.Close();
                currentAccountBalance -= transactionHb.Amount;

                var updateQuery = ("update blockchainaddress set balance = @balance where address = @address");
                var sqlCmd = new MySqlCommand(updateQuery, conn, myTransaction);
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);
                var updateRs = sqlCmd.ExecuteNonQuery();

                var balanceReceiver = new MySqlCommand("select * from blockchainaddress where address = @address ",
                    conn, myTransaction);
                balanceReceiver.Parameters.AddWithValue("@address", transactionHb.ReceiverAddress);
                double receiverBalance = 0;
                var readerReceiver = balanceReceiver.ExecuteReader();

                if (readerReceiver.Read())
                {
                    receiverBalance = readerReceiver.GetDouble("balance");
                }

                readerReceiver.Close();
                
                receiverBalance += (transactionHb.Amount * 200);


                var updateQueryReceiver =
                    ("update blockchainaddress set balance = @balance where address = @address");
                var sqlCmdReceiver = new MySqlCommand(updateQueryReceiver, conn, myTransaction);
                sqlCmdReceiver.Parameters.AddWithValue("@balance", receiverBalance);
                sqlCmdReceiver.Parameters.AddWithValue("@address", transactionHb.ReceiverAddress);
                var updateResultReceiver = sqlCmdReceiver.ExecuteNonQuery();

                var historyTransactionQuery =
                    "insert into blockchaintransaction (TransactionId, Type, SenderAddress, ReceiverAddress, Amount, CreatedAtMLS, UpdatedAtMLS, Status) " +
                    "values (@id, @type, @senderAddress, @receiverAddress, @amount, @createdAtMLS, @updatedAtMLS, @status)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, conn, myTransaction);
                historyTransactionCmd.Parameters.AddWithValue("@id", transactionHb.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@type", transactionHb.Type);
                historyTransactionCmd.Parameters.AddWithValue("@senderAddress",
                    transactionHb.SenderAddress);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAddress",
                    transactionHb.ReceiverAddress);
                historyTransactionCmd.Parameters.AddWithValue("@amount", transactionHb.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@createdAtMLS", transactionHb.CreatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@updatedAtMLS", transactionHb.UpdateAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@status", transactionHb.Status);
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
    }
}