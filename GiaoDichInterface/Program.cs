using System;
using System.Reflection.Metadata.Ecma335;
using GiaoDichInterface.entity;
using Microsoft.Win32.SafeHandles;

namespace GiaoDichInterface
{
    class Program
    {
        public static SHBAccount currentLoggedInAccount;
        public static BlockchainAddress CurrentAddress;
        
        static void Main(string[] args)
        {
            
            while (true)
            {
                Console.Clear();
                GiaoDich giaoDich = null;
                Console.WriteLine("Vui long lua chon kieu giao dich");
                Console.WriteLine("=============================");
                Console.WriteLine("1. Giao dich tren ngan hang SHB");
                Console.WriteLine("2. Giao dich tren ngan hang Blockchain");
                Console.WriteLine("===============");
                Console.WriteLine("Nhap lua chon: ");
                var choiceBank = int.Parse(Console.ReadLine());

                switch (choiceBank)
                {
                    case 1:
                        giaoDich = new SHB();
                        break;
                    case 2:
                        giaoDich = new BlockChain();
                        break;
                    default:
                        Console.WriteLine("Sai phuong thuc dang nhap");
                        break;
                }
                
                // yeu cau nguoi dung dang nhap
                giaoDich.Login();
                if (currentLoggedInAccount != null)
                {
                    Console.WriteLine("Dang nhap thanh cong voi tai khoan.");
                    Console.WriteLine($"Tai khoan: {currentLoggedInAccount.Username}");
                    Console.WriteLine($"So du: {currentLoggedInAccount.Balance}");
                    Console.WriteLine("Pls enter any press");
                    Console.ReadLine();
                    GenerateTransactionMenu(giaoDich);
                }
                
                if (CurrentAddress != null)
                {
                    Console.WriteLine("Dang nhap thanh cong voi tai khoan address.");
                    Console.WriteLine($"Tai khoan: {CurrentAddress.Address}");
                    Console.WriteLine($"So du: {CurrentAddress.Balance}");
                    Console.WriteLine("Pls enter any press");
                    Console.ReadLine();
                    GenerateTransactionMenu(giaoDich);
                }
            }
        }

        private static void GenerateTransactionMenu(GiaoDich giaoDich)
        {
            while (true)
            {
                Console.WriteLine("===========Choice==========");
                Console.WriteLine("1.Rut Tien");
                Console.WriteLine("2.Gui Tien");
                Console.WriteLine("3.Chuyen Tien");
                Console.WriteLine("4.Thoat");
                Console.WriteLine("+++++++++++++++++++++++++++");
                Console.WriteLine("Vui long chon : ");
                var choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        giaoDich.RutTien();
                        break;
                    case 2:
                        giaoDich.GuiTien();
                        break;
                    case 3:
                        giaoDich.ChuyenTien();
                        break;
                    case 4:
                        Console.WriteLine("Thoat giao dien giao dich");
                        break;
                    default:
                        Console.WriteLine("Sai phuong thuc nhap");
                        break;
                }

                if (choice == 4)
                {
                    break;
                }
            }
        }

        internal interface GiaoDich
        {
            void RutTien();
            void GuiTien();
            void ChuyenTien();
            void Login();
        }
    }
}